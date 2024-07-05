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
    using Microsoft.Azure.PowerShell.Ssh.Helpers.Compute.Models;
    using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;
    using System.Linq;
    using Microsoft.Rest.Azure;
    using System;
    using System.Management.Automation;

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
            Console.WriteLine("Creating Bastion");
            string temp = "north-europe-vm-vnet-bastion";
            BastionHost bastion = FetchDeveloperBastion(resourceGroupName, temp);
            Console.WriteLine(bastion);
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
            catch (Exception)
            {
                Console.WriteLine("Error");
                return null;

            }

        }

    }
}
#endregion