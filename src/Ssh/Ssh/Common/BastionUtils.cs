// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------


namespace Microsoft.Azure.Commands.Ssh
{
    using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
    using Microsoft.Azure.PowerShell.Ssh.Helpers.Network;


    using Microsoft.Azure.PowerShell.Ssh.Helpers.Network.Models;
    using Microsoft.Azure.PowerShell.Cmdlets.Ssh.AzureClients;
    using Microsoft.Azure.PowerShell.Ssh.Helpers.Compute;
    using Newtonsoft.Json;

    
    using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;
    using System.Linq;
    using Microsoft.Rest.Azure;
    using System;
    using System.Management.Automation;
    using System.Net;
    using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
    using System.Collections.Generic;
    using Microsoft.Azure.Commands.Common.Exceptions;
    using System.Reflection;
    using Microsoft.Azure.PowerShell.Cmdlets.Ssh.Common;
    using Microsoft.Azure.Management.WebSites.Version2016_09_01.Models;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using System.Management.Automation.Language;

    internal class BastionUtils
    {
        #region Fields
        private NetworkClient _networkClient;
        private IAzureContext _context;
        #endregion

        public BastionUtils(IAzureContext context)
        {
            _context = context;
        }

        #region Properties
        private NetworkClient NetworkClient
        {
            get
            {
                if (_networkClient == null)
                {
                    _networkClient = new NetworkClient(_context);
                }
                return _networkClient;
            }

            set { _networkClient = value; }
        }
        public IBastionHostsOperations BastionClient
        {
            get
            {
                return NetworkClient.NetworkManagementClient.BastionHosts;
            }
        }

        public void HandleBastionProperties( NetworkInterface nic, string resourceGroupName, string vmName, IAzureContext context)
        {

            string location = nic.Location;
            CheckValidBastionDeveloperLocation(location);

            string vmSubscriptionID = nic.VirtualMachine.Id;


            string vNetId = null;
            string vNetName = null;
            if (nic.IpConfigurations != null && nic.IpConfigurations.Any())
            {
                foreach (var ipConfig in nic.IpConfigurations)
                {
                    if (ipConfig.Subnet != null)
                    {
                        string subnetId = ipConfig.Subnet.Id;
                        
                        (vNetId, vNetName) = GetVNetDetailsFromSubnetId(subnetId);
                        break;
                    }
                }
            }
            ResourceGraphUtils resourceGraphUtils = new ResourceGraphUtils(context);
            string bastionsFoundInVNet = resourceGraphUtils.QueryResourceGraph(vNetId);
            string bastionNameInVNet = ParseAvailableBastions(bastionsFoundInVNet);

            BastionHost bastion;


            if (bastionNameInVNet == null)
            {
                string bastionName = resourceGroupName + "-vnet-bastion";

                bastion  = CreateDeveloperBastion(resourceGroupName, bastionName, location, vNetId);
                
            }
            else
            {
                bastion = FetchDeveloperBastion(resourceGroupName, bastionNameInVNet);

            }



        }

        public BastionHost FetchDeveloperBastion(string resourceGroupName, string name)
        {
            try
            {
                var bastion = this.BastionClient.Get(resourceGroupName, name);
                if (bastion.Sku.Name != "Developer")
                {
                    throw new InvalidOperationException("Only the Bastion Developer SKU is available");
                }

                return bastion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bastion: {ex.Message}");
                throw;
            }
        }

        public BastionHost CreateDeveloperBastion(string resourceGroupName, string bastionName, string location, string vNetId)
        {
            var virtualNetwork = new SubResource
            {
                Id = vNetId
            };

            var sku = new Sku
            {
                Name = "Developer"
            };

            var bastion = new BastionHost
            {
                Location = location,
                IpConfigurations = new List<BastionHostIPConfiguration>(), 
                VirtualNetwork = virtualNetwork,
                ScaleUnits = 2,
                DisableCopyPaste = null,
                EnableFileCopy = null,
                EnableIpConnect = null,
                EnableShareableLink = null,
                EnableTunneling = null,
                EnableKerberos = null,
                EnableSessionRecording = null,                
                Sku = sku,
                Tags = new Dictionary<string, string>()
            };

            try
            {
                this.BastionClient.CreateOrUpdate(resourceGroupName, bastionName, bastion);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating bastion: {ex.Message}");
                throw;
            }
            return this.BastionClient.Get(resourceGroupName, bastionName);
        }

        protected void CheckValidBastionDeveloperLocation(string location)
        {
            string[] validLocations = { "centralus", "eastus2", "westus", "northeurope", "northcentralus", "westcentralus" };

            if (!Array.Exists(validLocations, element => element.Equals(location, StringComparison.OrdinalIgnoreCase)))
            {
                string error = ($"The Bastion Developer Sku is not currently available in the specified region." +
                    $"Learn more here: https://learn.microsoft.com/en-us/azure/bastion/configuration-settings");
                throw new AzPSCloudException(error);

            }
        }
        private (string vNetId, string vNetName) GetVNetDetailsFromSubnetId(string subnetId)
        {
            var parts = subnetId.Split('/');
            int vNetIndex = Array.IndexOf(parts, "virtualNetworks");
            if (vNetIndex == -1 || vNetIndex >= parts.Length - 1)
            {
                throw new ArgumentException("Invalid subnet ID", nameof(subnetId));
            }
            string vNetName = parts[vNetIndex + 1];
            string vNetId = string.Join("/", parts.Take(vNetIndex + 2));
            return (vNetId, vNetName);
        }

        public static string ParseAvailableBastions(string bastionsFoundInVnet)
        {
            var jsonObject = JObject.Parse(bastionsFoundInVnet);

            int count = (int)jsonObject["count"];

            if (count == 0)
            {
                return null;
            }

            if (count >= 1)
            {
                var bastionsArray = jsonObject["data"];
                if (bastionsArray != null && bastionsArray.HasValues)
                {
                    var firstBastion = bastionsArray.First;
                    if (firstBastion != null && firstBastion["name"] != null)
                    {
                        return firstBastion["name"].ToString();
                    }
                }
            }

            return null;
        }
    

    private async Task<string> GetDataPodEndPoint(BastionHost bastion, IAzureContext context, string virtualMachineId, int resourcePort)
        {
            //var authToken = await GetAuthToken(context); 

            var content = new
            {
                resourceId = virtualMachineId,
                bastionResourceId = bastion.Id,
                vmPort = resourcePort,
               // azToken = authToken,
                connectionType = "nativeclient"
            };
            
            var jsonContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var webAddress = $"https://{bastion.DnsName}/api/connection"; 

            using (var client = new HttpClient())
            {
                
                client.DefaultRequestHeaders.Add("Connection", "close");
                client.DefaultRequestHeaders.Add("User-Agent", "PowerShell");
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                

                HttpResponseMessage response = await client.PostAsync(webAddress, stringContent);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}
#endregion