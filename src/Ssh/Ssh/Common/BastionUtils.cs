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



using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.Common.Exceptions;
using Microsoft.Azure.PowerShell.Cmdlets.Ssh.AzureClients;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Compute;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.PowerShell.Cmdlets.Ssh.Common

{
    internal class BastionUtils
    {
        #region Fields
        private NetworkClient _networkClient;
        private IAzureContext _context;
        public const string _bastionDevloperSku = "Developer";

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
        #endregion
        public int HandleBastionProperties(NetworkInterface nic, string vmName, IAzureContext context, string vmPort, Process sshProcess)
        {
            
            string location;
            int port = 22;
            if (CheckValidBastionDeveloperLocation(nic.Location) == false)
            {
                string error = ($"The Bastion Developer Sku is not currently available in the specified region." +
                     $"Learn more here: https://learn.microsoft.com/en-us/azure/bastion/configuration-settings");
                throw new AzPSCloudException(error);
            }
            location = nic.Location;

            string vmSubscriptionID = nic.VirtualMachine.Id;


            string vNetId = null;
            string vNetName = null;
            string vNetResourceGroup = null;
            if (nic.IpConfigurations != null && nic.IpConfigurations.Any())
            {
                foreach (var ipConfig in nic.IpConfigurations)
                {
                    if (ipConfig.Subnet != null)
                    {
                        string subnetId = ipConfig.Subnet.Id;

                        (vNetId, vNetName, vNetResourceGroup) = GetVNetDetailsFromSubnetId(subnetId);
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
                string bastionName = vNetName + "-bastion";
                bastion = CreateDeveloperBastion(vNetResourceGroup, bastionName, location, vNetId);

            }
            else
            {
                bastion = FetchDeveloperBastion(vNetResourceGroup, bastionNameInVNet);

            }
            int bastionPort = 0;  
            string bastionEndPoint = null;
            try
            {
                bastionEndPoint = GetDataPodEndPoint(bastion, context, vmSubscriptionID, port);

            }
            catch (Exception ex)
            {
                throw new AzPSCloudException($"Error fetching bastion end point: {ex.Message}");
            }

            try
            {
                TunnelServer tunnel = new TunnelServer(context, port, bastion, bastionEndPoint, vmSubscriptionID, bastionPort);

                Thread tunnelThread = new Thread(() => tunnel.StartServer());
                tunnelThread.Start();

                sshProcess.Start();
                sshProcess.WaitForExit();
                int sshExitCode = sshProcess.ExitCode;

                return sshExitCode;
            }
            catch (Exception ex)
            {
                string error = "Error connecting to Bastion Host: " + ex.Message;
                throw new AzPSCloudException(error);
            }

        }

        public BastionHost FetchDeveloperBastion(string resourceGroupName, string name)
        {
            try
            {
                var bastion = this.BastionClient.Get(resourceGroupName, name);
                if (bastion.Sku.Name != _bastionDevloperSku)
                {
                    throw new InvalidOperationException("SSH to Bastion host is only support for Developer Bastion Skus");
                }

                return bastion;
            }
            catch (Exception ex)
            {

                throw new AzPSCloudException($"An error occurred while fetching bastion host: {ex.Message}");
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
                Name = _bastionDevloperSku
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
                throw new AzPSCloudException($"Error creating bastion: {ex.Message}");
            }
            return this.BastionClient.Get(resourceGroupName, bastionName);
        }

        protected bool CheckValidBastionDeveloperLocation(string location)
        {
            string[] validLocations = { "centraluseuap", "eastus2euap", "westus", "northeurope", "northcentralus", "westcentralus" };

            if (!Array.Exists(validLocations, element => element.Equals(location, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }
        private (string vNetId, string vNetName, string resourceGroupName) GetVNetDetailsFromSubnetId(string subnetId)
        {
            var parts = subnetId.Split('/');
            int vNetIndex = Array.IndexOf(parts, "virtualNetworks");
            int resourceGroupIndex = Array.IndexOf(parts, "resourceGroups");

            if (vNetIndex == -1 || vNetIndex >= parts.Length - 1)
            {
                throw new ArgumentException("Invalid subnet ID", nameof(subnetId));
            }
            if (resourceGroupIndex == -1 || resourceGroupIndex >= parts.Length - 1)
            {
                throw new ArgumentException("Invalid subnet ID", nameof(subnetId));
            }

            string vNetName = parts[vNetIndex + 1];
            string vNetId = string.Join("/", parts.Take(vNetIndex + 2));
            string resourceGroupName = parts[resourceGroupIndex + 1];

            return (vNetId, vNetName, resourceGroupName);
        }

        public static string ParseAvailableBastions(string bastionsFoundInVnet)
        {
            var jsonObject = JObject.Parse(bastionsFoundInVnet);

            int count = (int)jsonObject["count"];

            if (count == 0)
            {
                return null;
            }

            var bastion = jsonObject["data"].First;
            if (bastion != null && bastion["sku"] != null && bastion["sku"]["name"] != null)
            {
                string skuName = bastion["sku"]["name"].ToString();
                if (skuName != _bastionDevloperSku)
                {
                    throw new InvalidOperationException("SSH to Bastion host is only support for Developer Bastion Skus");

                }
                if (bastion["name"] != null)
                {
                    return bastion["name"].ToString();
                }
            }

            return null;
        }



        public string GetDataPodEndPoint(BastionHost bastion, IAzureContext context, string virtualMachineId, int resourcePort)
        {
            string accessToken = context.Account.GetAccessToken();
            var content = new
            {
                resourceId = virtualMachineId,
                bastionResourceId = bastion.Id,
                vmPort = resourcePort,
                azToken = accessToken,
                connectionType = "nativeclient"
            };

            var jsonContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var webAddress = $"https://{bastion.DnsName}/api/connection";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Connection", "close");
                client.DefaultRequestHeaders.Add("User-Agent", "PowerShell");

                HttpResponseMessage response = client.PostAsync(webAddress, stringContent).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return responseBody;
            }
        }

    }
}
