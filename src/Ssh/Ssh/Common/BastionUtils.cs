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
    using System.Threading;
    using Newtonsoft.Json.Linq;
    using System.Management.Automation.Language;
    using System.Diagnostics;

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
        #endregion
        public void HandleBastionProperties(NetworkInterface nic, string resourceGroupName, string vmName, IAzureContext context, string vmPort, Process sshProcess)
        {
            // if (vmPort != "22" || vmPort != null)
            // {
            //throw new InvalidOperationException("SSH to Bastion host is only support for Developer Bastion Skus");
            // }
            int port = 22;
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

                bastion = CreateDeveloperBastion(resourceGroupName, bastionName, location, vNetId);

            }
            else
            {
                bastion = FetchDeveloperBastion(resourceGroupName, bastionNameInVNet);

            }
            int bastionPort = 0;  // no custom ports for the bastion developer
            string bastionEndPoint = null;
            try
            {


                bastionEndPoint = GetDataPodEndPoint(bastion, context, vmSubscriptionID, port);
                Console.WriteLine(bastionEndPoint);

                
            }
            catch (Exception ex)
            {
                string error = "Error fetching end point: " + ex.Message;
                throw new AzPSCloudException(error);
            }

            try {
                TunnelServer tunnel = new TunnelServer(context, port, bastion, bastionEndPoint, vmSubscriptionID, bastionPort);

                Thread tunnelThread = new Thread(() => tunnel.StartServer());
                tunnelThread.Start();
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
                if (bastion.Sku.Name != "Developer")
                {
                    throw new InvalidOperationException("SSH to Bastion host is only support for Developer Bastion Skus");
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

            var bastion = jsonObject["data"].First;
            if (bastion != null && bastion["sku"] != null && bastion["sku"]["name"] != null)
            {
                string skuName = bastion["sku"]["name"].ToString();
                if (skuName != "Developer")
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
            //string accessToken= context.Account.GetAccessToken();
            var content = new
            {
                resourceId = virtualMachineId,
                bastionResourceId = bastion.Id,
                vmPort = resourcePort,
                azToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyIsImtpZCI6Ik1HTHFqOThWTkxvWGFGZnBKQ0JwZ0I0SmFLcyJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNzIwODA1OTQ4LCJuYmYiOjE3MjA4MDU5NDgsImV4cCI6MTcyMDgxMDA4MiwiX2NsYWltX25hbWVzIjp7Imdyb3VwcyI6InNyYzEifSwiX2NsYWltX3NvdXJjZXMiOnsic3JjMSI6eyJlbmRwb2ludCI6Imh0dHBzOi8vZ3JhcGgud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3L3VzZXJzLzRmNGM0MDhkLTVjOGQtNGJhYS05MmExLTlhZDIyZTZkMTZkYy9nZXRNZW1iZXJPYmplY3RzIn19LCJhY3IiOiIxIiwiYWlvIjoiQVpRQWEvOFhBQUFBaXJSZHJNQWIxT0FZcEwxN2c3c1gvR21GcTFackRJUXJLaUdvUW8zZ3hSSmoxYjU2M2lraE0yb2VEcjg3S3d1b2had0J0RFZxUmJUSW1hSFJDZ1BqMFVoOFJFMXRMQnNxM3h5YjY5eHNZNUxqZ1VUVVFWeWhhQm50by8xYWY4QStUWFFFdXkwMEZBdXpHUHRsbEdLdjFTREM2SXdsemZ2NU5wcUJmQjhUZ1U4bzNLU3FOSjlwZ3Fkczh2RFFML0lEIiwiYW1yIjpbInJzYSIsIm1mYSJdLCJhcHBpZCI6IjA0YjA3Nzk1LThkZGItNDYxYS1iYmVlLTAyZjllMWJmN2I0NiIsImFwcGlkYWNyIjoiMCIsImNhcG9saWRzX2xhdGViaW5kIjpbIjI5Mzk5Y2Y5LTliNmItNDIwNS1iNWIzLTEzYTEzNGU5YjIzMyJdLCJkZXZpY2VpZCI6IjljNTU3Mjc3LTcwODYtNDg1My04NjI5LTg1NTVkMjY2NTg3MyIsImZhbWlseV9uYW1lIjoiQ29iYWxlZGEiLCJnaXZlbl9uYW1lIjoiTGVvbmFyZG8iLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIyMDAxOjQ4OTg6YTgwMDoxMDEwOjMyNDU6YTlhNTplZWRjOjQ5ZDQiLCJuYW1lIjoiTGVvbmFyZG8gQ29iYWxlZGEiLCJvaWQiOiI0ZjRjNDA4ZC01YzhkLTRiYWEtOTJhMS05YWQyMmU2ZDE2ZGMiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjEyNzUyMTE4NC0xNjA0MDEyOTIwLTE4ODc5Mjc1MjctNzY2NTE3NTAiLCJwdWlkIjoiMTAwMzIwMDM3QTBGQTlBOCIsInJoIjoiMC5BUm9BdjRqNWN2R0dyMEdScXkxODBCSGJSMFpJZjNrQXV0ZFB1a1Bhd2ZqMk1CTWFBSlkuIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiWXBJeVNtZC13b0JCSzQyVVd0M3ZXajduSFNMdlhvdlBzZU52b0Ezck9mMCIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoidC1sY29iYWxlZGFAbWljcm9zb2Z0LmNvbSIsInVwbiI6InQtbGNvYmFsZWRhQG1pY3Jvc29mdC5jb20iLCJ1dGkiOiJRdHFteFlCZEhrUzZ2c0tlRHRJV0FBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX2NhZSI6IjEiLCJ4bXNfY2MiOlsiQ1AxIl0sInhtc19maWx0ZXJfaW5kZXgiOlsiMjYiXSwieG1zX2lkcmVsIjoiOCAxIiwieG1zX3JkIjoiMC40MkxsWUJSaWxBSUEiLCJ4bXNfc3NtIjoiMSIsInhtc190Y2R0IjoxMjg5MjQxNTQ3fQ.VPEnkbKnJg2gwVWhucPxx2sJoYcsIvxxJ6FLOC43id5rb9Uri_GIJRVwY120cTjVIB3SViVAUa2kfPJZksyTkKkPH0zv7QYrHwE1WC1zP5CQeGHYgmL9tw8YU4lKB5ZtpTET7KHz8VOF9wvamLn5TrBQ1pD-7hYYbbkm0H2ibJfff1kJ-eJ0M9hWCEcH26k00U0PihxvhsN8YSWgO4SHyyUrPYS7eSOn9kJOR1rnyi1NYB5tfCDNvzYBJ3x2ZwuNMVRAOVzVqbrR49UrnMwTcQIWXfhu89yY_gjSpItTKmSLBghClfkddHjz-3kZpHtbx-iYCyErSowak-NyLDtyAQ",
                //azToken = accessToken,
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
