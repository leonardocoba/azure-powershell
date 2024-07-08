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
        
        public void HandleBastionProperties(string resourceGroupName, string bastionName, IAzureContext context)
        {
            Console.WriteLine("Handling Bastion Properties");
            string vm = "north-europe-vm-vnet-bastion";
            string rsgroup = "north-europe-vm-vnet-bastion";
            BastionHost bastion = null;

            try
            {
                bastion = FetchDeveloperBastion(resourceGroupName, vm);
                Console.WriteLine($"Bastion: {bastion}");
                Console.WriteLine("Bastion found with VM name.");
            }
            catch (Rest.Azure.CloudException exception)
            {
                if (exception.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Bastion not found with VM name, trying with resource group name.");
                }
                else
                {
                    throw; // Rethrow if it's an unexpected exception
                }
            }

            if (bastion == null)
            {
                try
                {
                    bastion = FetchDeveloperBastion(resourceGroupName, rsgroup);
                    Console.WriteLine("Bastion found with resource group name.");
                }
                catch (Rest.Azure.CloudException exception)
                {
                    if (exception.Response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Bastion not found with resource group name, creating new bastion.");
                    }
                    else
                    {
                        throw; // Rethrow if it's an unexpected exception
                    }
                }
            }

            if (bastion == null)
            {
                CreateDeveloperBastion(resourceGroupName, bastionName);
                Console.WriteLine("Bastion created.");
            }

            string json = JsonConvert.SerializeObject(bastion, Formatting.Indented);
            Console.WriteLine("JSON View: ");
            Console.WriteLine(json);
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

        public BastionHost CreateDeveloperBastion(string resourceGroupName, string bastionName)
        {
            var bastionProperties = new
            {
                ipConfigurations = new List<object>(), // Empty list for IP configurations
                dnsName = "omnibrain.northeurope.bastionglobal.azure.com",
                virtualNetwork = new
                {
                    id = "/subscriptions/f02fce97-0b41-4430-9823-6c1545921419/resourceGroups/Bastion-Dev-Testing/providers/Microsoft.Network/virtualNetworks/north-europe-vm-vnet"
                },
                scaleUnits = 2,
                disableCopyPaste = (object)null,
                enableFileCopy = (object)null,
                enableIpConnect = (object)null,
                enableShareableLink = (object)null,
                enableTunneling = (object)null,
                enableKerberos = (object)null,
                enableSessionRecording = (object)null,
                sku = new { name = "Developer" }
            };

            var bastion = new
            {
                properties = bastionProperties,
                id = "/subscriptions/f02fce97-0b41-4430-9823-6c1545921419/resourceGroups/Bastion-Dev-Testing/providers/Microsoft.Network/bastionHosts/north-europe-vm-vnet-bastion",
                name = "north-europe-vm-vnet-bastion",
                type = "Microsoft.Network/bastionHosts",
                location = "northeurope",
                tags = new Dictionary<string, string>()
            };

            // Map to the SDK object
            var bastionModel = NetworkResourceManagerProfile.Mapper.Map<BastionHost>(bastion);

            // Execute the Create bastion call
            this.BastionClient.CreateOrUpdate(resourceGroupName, bastionName, bastionModel);

            // Return the created bastion
            return this.BastionClient.Get(resourceGroupName, bastionName);
        }


    }
}
#endregion