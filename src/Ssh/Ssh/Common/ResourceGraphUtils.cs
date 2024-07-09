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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.Common.Exceptions;
using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
using Microsoft.Azure.Management.Internal.Network.Common;
using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;
using Microsoft.Azure.Management.WebSites.Version2016_09_01.Models;
using Microsoft.Azure.PowerShell.Cmdlets.Ssh.AzureClients;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Compute;
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network;
using Microsoft.Azure.PowerShell.Ssh.Helpers.ResourceGraph.Models;
using Microsoft.Rest.Azure;
using Newtonsoft.Json;

namespace Microsoft.Azure.PowerShell.Cmdlets.Ssh.Common
{
    internal class ResourceGraphUtils
    {
        private IResourceGraphClient _resourceGraphClient;
        private IAzureContext _context;

        public ResourceGraphUtils(IAzureContext context)
        {
            _context = context;
        }

        private IResourceGraphClient ResourceGraphClient
        {
            get
            {
                if (_resourceGraphClient == null)
                {
                    _resourceGraphClient = new ResourceGraphClient(_context);
                    // Ensure that ResourceGraphClient is correctly instantiated, 'new IResourceGraphClient' is invalid
                }
                return _resourceGraphClient;
            }
            set
            {
                _resourceGraphClient = value;
            }
        }

        public string QueryResourceGraph(string vNetID)
        {
            string query = $"Resources | where type =~ 'Microsoft.Network/bastionHosts' and (properties.ipConfigurations[0].properties.subnet.id startswith '{vNetID}/' or properties.virtualNetwork.id =~ '{vNetID}') " +
                "| project id, location, name, sku, properties, type, vnetid = '{vNetID}' " +
                "| union (Resources | where id =~ '{vNetID}' " +
                "| mv-expand peering = properties.virtualNetworkPeerings limit 400 " +
                "| project vnetid = tolower(tostring(peering.properties.remoteVirtualNetwork.id)) " +
                "| join kind = inner (Resources | where type =~ 'microsoft.network/bastionHosts' " +
                "| extend vnetid = tolower(extract('(.*/virtualnetworks/[^/]+)/', 1, tolower(tostring(properties.ipConfigurations[0].properties.subnet.id))))) on vnetid)";

            QueryResponse response = null;
            try
            {
                QueryRequest request = new QueryRequest
                {
                    Query = query
                };
                response = ResourceGraphClient.ResourcesWithHttpMessagesAsync(request).Result.Body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing Resource Graph query: {ex.Message}");
                return null;
            }

            if (response.ResultTruncated == ResultTruncated.True)
            {
                Console.WriteLine("Warning: Result is truncated.");
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
