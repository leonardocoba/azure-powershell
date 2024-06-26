// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.Functions.Models.Api20231201
{
    using static Microsoft.Azure.PowerShell.Cmdlets.Functions.Runtime.Extensions;

    /// <summary>Site seal</summary>
    public partial class SiteSeal :
        Microsoft.Azure.PowerShell.Cmdlets.Functions.Models.Api20231201.ISiteSeal,
        Microsoft.Azure.PowerShell.Cmdlets.Functions.Models.Api20231201.ISiteSealInternal
    {

        /// <summary>Backing field for <see cref="Html" /> property.</summary>
        private string _html;

        /// <summary>HTML snippet</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Functions.Origin(Microsoft.Azure.PowerShell.Cmdlets.Functions.PropertyOrigin.Owned)]
        public string Html { get => this._html; set => this._html = value; }

        /// <summary>Creates an new <see cref="SiteSeal" /> instance.</summary>
        public SiteSeal()
        {

        }
    }
    /// Site seal
    public partial interface ISiteSeal :
        Microsoft.Azure.PowerShell.Cmdlets.Functions.Runtime.IJsonSerializable
    {
        /// <summary>HTML snippet</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.Functions.Runtime.Info(
        Required = true,
        ReadOnly = false,
        Description = @"HTML snippet",
        SerializedName = @"html",
        PossibleTypes = new [] { typeof(string) })]
        string Html { get; set; }

    }
    /// Site seal
    internal partial interface ISiteSealInternal

    {
        /// <summary>HTML snippet</summary>
        string Html { get; set; }

    }
}