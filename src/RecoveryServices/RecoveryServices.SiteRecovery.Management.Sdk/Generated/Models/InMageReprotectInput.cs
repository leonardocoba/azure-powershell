// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.Management.RecoveryServices.SiteRecovery.Models
{
    using System.Linq;

    /// <summary>
    /// InMageAzureV2 specific provider input.
    /// </summary>
    [Newtonsoft.Json.JsonObject("InMage")]
    public partial class InMageReprotectInput : ReverseReplicationProviderSpecificInput
    {
        /// <summary>
        /// Initializes a new instance of the InMageReprotectInput class.
        /// </summary>
        public InMageReprotectInput()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InMageReprotectInput class.
        /// </summary>

        /// <param name="masterTargetId">The Master Target Id.
        /// </param>

        /// <param name="processServerId">The Process Server Id.
        /// </param>

        /// <param name="retentionDrive">The retention drive to use on the MT.
        /// </param>

        /// <param name="runAsAccountId">The CS account Id.
        /// </param>

        /// <param name="datastoreName">The target datastore name.
        /// </param>

        /// <param name="diskExclusionInput">The enable disk exclusion input.
        /// </param>

        /// <param name="profileId">The Policy Id.
        /// </param>

        /// <param name="disksToInclude">The disks to include list.
        /// </param>
        public InMageReprotectInput(string masterTargetId, string processServerId, string retentionDrive, string profileId, string runAsAccountId = default(string), string datastoreName = default(string), InMageDiskExclusionInput diskExclusionInput = default(InMageDiskExclusionInput), System.Collections.Generic.IList<string> disksToInclude = default(System.Collections.Generic.IList<string>))

        {
            this.MasterTargetId = masterTargetId;
            this.ProcessServerId = processServerId;
            this.RetentionDrive = retentionDrive;
            this.RunAsAccountId = runAsAccountId;
            this.DatastoreName = datastoreName;
            this.DiskExclusionInput = diskExclusionInput;
            this.ProfileId = profileId;
            this.DisksToInclude = disksToInclude;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();


        /// <summary>
        /// Gets or sets the Master Target Id.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "masterTargetId")]
        public string MasterTargetId {get; set; }

        /// <summary>
        /// Gets or sets the Process Server Id.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "processServerId")]
        public string ProcessServerId {get; set; }

        /// <summary>
        /// Gets or sets the retention drive to use on the MT.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "retentionDrive")]
        public string RetentionDrive {get; set; }

        /// <summary>
        /// Gets or sets the CS account Id.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "runAsAccountId")]
        public string RunAsAccountId {get; set; }

        /// <summary>
        /// Gets or sets the target datastore name.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "datastoreName")]
        public string DatastoreName {get; set; }

        /// <summary>
        /// Gets or sets the enable disk exclusion input.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "diskExclusionInput")]
        public InMageDiskExclusionInput DiskExclusionInput {get; set; }

        /// <summary>
        /// Gets or sets the Policy Id.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "profileId")]
        public string ProfileId {get; set; }

        /// <summary>
        /// Gets or sets the disks to include list.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "disksToInclude")]
        public System.Collections.Generic.IList<string> DisksToInclude {get; set; }
        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (this.MasterTargetId == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "MasterTargetId");
            }
            if (this.ProcessServerId == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "ProcessServerId");
            }
            if (this.RetentionDrive == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "RetentionDrive");
            }
            if (this.ProfileId == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "ProfileId");
            }








        }
    }
}