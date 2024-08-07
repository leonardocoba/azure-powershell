// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview
{
    using static Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Extensions;

    /// <summary>Object that describes a specific usage of the resources.</summary>
    public partial class SignalRServiceUsage :
        Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsage,
        Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageInternal
    {

        /// <summary>Backing field for <see cref="CurrentValue" /> property.</summary>
        private long? _currentValue;

        /// <summary>Current value for the usage quota.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Owned)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.FormatTable(Index = 1)]
        public long? CurrentValue { get => this._currentValue; set => this._currentValue = value; }

        /// <summary>Backing field for <see cref="Id" /> property.</summary>
        private string _id;

        /// <summary>Fully qualified ARM resource id</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Owned)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.DoNotFormat]
        public string Id { get => this._id; set => this._id = value; }

        /// <summary>Backing field for <see cref="Limit" /> property.</summary>
        private long? _limit;

        /// <summary>
        /// The maximum permitted value for the usage quota. If there is no limit, this value will be -1.
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Owned)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.FormatTable(Index = 2)]
        public long? Limit { get => this._limit; set => this._limit = value; }

        /// <summary>Internal Acessors for Name</summary>
        Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageName Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageInternal.Name { get => (this._name = this._name ?? new Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.SignalRServiceUsageName()); set { {_name = value;} } }

        /// <summary>Backing field for <see cref="Name" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageName _name;

        /// <summary>Localizable String object containing the name and a localized value.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Owned)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.DoNotFormat]
        internal Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageName Name { get => (this._name = this._name ?? new Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.SignalRServiceUsageName()); set => this._name = value; }

        /// <summary>Localized name of the usage.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Inlined)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.DoNotFormat]
        public string NameLocalizedValue { get => ((Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageNameInternal)Name).LocalizedValue; set => ((Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageNameInternal)Name).LocalizedValue = value ?? null; }

        /// <summary>The identifier of the usage.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Inlined)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.FormatTable(Index = 0)]
        public string NameValue { get => ((Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageNameInternal)Name).Value; set => ((Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageNameInternal)Name).Value = value ?? null; }

        /// <summary>Backing field for <see cref="Unit" /> property.</summary>
        private string _unit;

        /// <summary>
        /// Representing the units of the usage quota. Possible values are: Count, Bytes, Seconds, Percent, CountPerSecond, BytesPerSecond.
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Origin(Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.PropertyOrigin.Owned)]
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.FormatTable(Index = 3)]
        public string Unit { get => this._unit; set => this._unit = value; }

        /// <summary>Creates an new <see cref="SignalRServiceUsage" /> instance.</summary>
        public SignalRServiceUsage()
        {

        }
    }
    /// Object that describes a specific usage of the resources.
    public partial interface ISignalRServiceUsage :
        Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.IJsonSerializable
    {
        /// <summary>Current value for the usage quota.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Current value for the usage quota.",
        SerializedName = @"currentValue",
        PossibleTypes = new [] { typeof(long) })]
        long? CurrentValue { get; set; }
        /// <summary>Fully qualified ARM resource id</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Fully qualified ARM resource id",
        SerializedName = @"id",
        PossibleTypes = new [] { typeof(string) })]
        string Id { get; set; }
        /// <summary>
        /// The maximum permitted value for the usage quota. If there is no limit, this value will be -1.
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The maximum permitted value for the usage quota. If there is no limit, this value will be -1.",
        SerializedName = @"limit",
        PossibleTypes = new [] { typeof(long) })]
        long? Limit { get; set; }
        /// <summary>Localized name of the usage.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Localized name of the usage.",
        SerializedName = @"localizedValue",
        PossibleTypes = new [] { typeof(string) })]
        string NameLocalizedValue { get; set; }
        /// <summary>The identifier of the usage.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The identifier of the usage.",
        SerializedName = @"value",
        PossibleTypes = new [] { typeof(string) })]
        string NameValue { get; set; }
        /// <summary>
        /// Representing the units of the usage quota. Possible values are: Count, Bytes, Seconds, Percent, CountPerSecond, BytesPerSecond.
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Representing the units of the usage quota. Possible values are: Count, Bytes, Seconds, Percent, CountPerSecond, BytesPerSecond.",
        SerializedName = @"unit",
        PossibleTypes = new [] { typeof(string) })]
        string Unit { get; set; }

    }
    /// Object that describes a specific usage of the resources.
    internal partial interface ISignalRServiceUsageInternal

    {
        /// <summary>Current value for the usage quota.</summary>
        long? CurrentValue { get; set; }
        /// <summary>Fully qualified ARM resource id</summary>
        string Id { get; set; }
        /// <summary>
        /// The maximum permitted value for the usage quota. If there is no limit, this value will be -1.
        /// </summary>
        long? Limit { get; set; }
        /// <summary>Localizable String object containing the name and a localized value.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.WebPubSub.Models.Api20220801Preview.ISignalRServiceUsageName Name { get; set; }
        /// <summary>Localized name of the usage.</summary>
        string NameLocalizedValue { get; set; }
        /// <summary>The identifier of the usage.</summary>
        string NameValue { get; set; }
        /// <summary>
        /// Representing the units of the usage quota. Possible values are: Count, Bytes, Seconds, Percent, CountPerSecond, BytesPerSecond.
        /// </summary>
        string Unit { get; set; }

    }
}