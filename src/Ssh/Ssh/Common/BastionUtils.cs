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
using Microsoft.Azure.PowerShell.Ssh.Helpers.Network;

using Microsoft.Azure.PowerShell.Ssh.Helpers.Network.Models;
using System;

namespace Microsoft.Azure.Commands.Ssh
{
    public abstract class BastionUtils
    {
        }
        public static void HandleBastionProperties(string resourceGroupName, string name, IAzureContext context)
        {
            Console.WriteLine("Initialized");
            BastionHost bastion = CreateDeveloperBastion(resourceGroupName, name);
        }

        public BastionHost CreateDeveloperBastion(string resourceGroupName, string name )
        {
            var bastion = this.BastionClient.Get(resourceGroupName, name);

            if (bastion.Sku.Name != "Developer")
            {
                throw new InvalidOperationException("Only the Bastion Developer SKU is available");
            }
            return bastion;
        }
    }
}
