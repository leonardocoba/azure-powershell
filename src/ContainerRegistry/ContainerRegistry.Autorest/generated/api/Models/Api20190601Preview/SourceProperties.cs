// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview
{
    using static Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Extensions;

    /// <summary>The properties of the source code repository.</summary>
    public partial class SourceProperties :
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.ISourceProperties,
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.ISourcePropertiesInternal
    {

        /// <summary>Backing field for <see cref="Branch" /> property.</summary>
        private string _branch;

        /// <summary>The branch name of the source code.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Owned)]
        public string Branch { get => this._branch; set => this._branch = value; }

        /// <summary>Internal Acessors for SourceControlAuthProperty</summary>
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfo Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.ISourcePropertiesInternal.SourceControlAuthProperty { get => (this._sourceControlAuthProperty = this._sourceControlAuthProperty ?? new Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.AuthInfo()); set { {_sourceControlAuthProperty = value;} } }

        /// <summary>Backing field for <see cref="RepositoryUrl" /> property.</summary>
        private string _repositoryUrl;

        /// <summary>The full URL to the source code repository</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Owned)]
        public string RepositoryUrl { get => this._repositoryUrl; set => this._repositoryUrl = value; }

        /// <summary>Backing field for <see cref="SourceControlAuthProperty" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfo _sourceControlAuthProperty;

        /// <summary>
        /// The authorization properties for accessing the source code repository and to set up
        /// webhooks for notifications.
        /// </summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Owned)]
        internal Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfo SourceControlAuthProperty { get => (this._sourceControlAuthProperty = this._sourceControlAuthProperty ?? new Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.AuthInfo()); set => this._sourceControlAuthProperty = value; }

        /// <summary>Time in seconds that the token remains valid</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Inlined)]
        public int? SourceControlAuthPropertyExpiresIn { get => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).ExpiresIn; set => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).ExpiresIn = value ?? default(int); }

        /// <summary>The refresh token used to refresh the access token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Inlined)]
        public string SourceControlAuthPropertyRefreshToken { get => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).RefreshToken; set => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).RefreshToken = value ?? null; }

        /// <summary>The scope of the access token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Inlined)]
        public string SourceControlAuthPropertyScope { get => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).Scope; set => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).Scope = value ?? null; }

        /// <summary>The access token used to access the source control provider.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Inlined)]
        public string SourceControlAuthPropertyToken { get => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).Token; set => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).Token = value ?? null; }

        /// <summary>The type of Auth token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Inlined)]
        public Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.TokenType? SourceControlAuthPropertyTokenType { get => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).TokenType; set => ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfoInternal)SourceControlAuthProperty).TokenType = value ?? ((Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.TokenType)""); }

        /// <summary>Backing field for <see cref="SourceControlType" /> property.</summary>
        private Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.SourceControlType _sourceControlType;

        /// <summary>The type of source control service.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Origin(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.PropertyOrigin.Owned)]
        public Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.SourceControlType SourceControlType { get => this._sourceControlType; set => this._sourceControlType = value; }

        /// <summary>Creates an new <see cref="SourceProperties" /> instance.</summary>
        public SourceProperties()
        {

        }
    }
    /// The properties of the source code repository.
    public partial interface ISourceProperties :
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.IJsonSerializable
    {
        /// <summary>The branch name of the source code.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The branch name of the source code.",
        SerializedName = @"branch",
        PossibleTypes = new [] { typeof(string) })]
        string Branch { get; set; }
        /// <summary>The full URL to the source code repository</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = true,
        ReadOnly = false,
        Description = @"The full URL to the source code repository",
        SerializedName = @"repositoryUrl",
        PossibleTypes = new [] { typeof(string) })]
        string RepositoryUrl { get; set; }
        /// <summary>Time in seconds that the token remains valid</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"Time in seconds that the token remains valid",
        SerializedName = @"expiresIn",
        PossibleTypes = new [] { typeof(int) })]
        int? SourceControlAuthPropertyExpiresIn { get; set; }
        /// <summary>The refresh token used to refresh the access token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The refresh token used to refresh the access token.",
        SerializedName = @"refreshToken",
        PossibleTypes = new [] { typeof(string) })]
        string SourceControlAuthPropertyRefreshToken { get; set; }
        /// <summary>The scope of the access token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The scope of the access token.",
        SerializedName = @"scope",
        PossibleTypes = new [] { typeof(string) })]
        string SourceControlAuthPropertyScope { get; set; }
        /// <summary>The access token used to access the source control provider.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The access token used to access the source control provider.",
        SerializedName = @"token",
        PossibleTypes = new [] { typeof(string) })]
        string SourceControlAuthPropertyToken { get; set; }
        /// <summary>The type of Auth token.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = false,
        ReadOnly = false,
        Description = @"The type of Auth token.",
        SerializedName = @"tokenType",
        PossibleTypes = new [] { typeof(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.TokenType) })]
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.TokenType? SourceControlAuthPropertyTokenType { get; set; }
        /// <summary>The type of source control service.</summary>
        [Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Runtime.Info(
        Required = true,
        ReadOnly = false,
        Description = @"The type of source control service.",
        SerializedName = @"sourceControlType",
        PossibleTypes = new [] { typeof(Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.SourceControlType) })]
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.SourceControlType SourceControlType { get; set; }

    }
    /// The properties of the source code repository.
    internal partial interface ISourcePropertiesInternal

    {
        /// <summary>The branch name of the source code.</summary>
        string Branch { get; set; }
        /// <summary>The full URL to the source code repository</summary>
        string RepositoryUrl { get; set; }
        /// <summary>
        /// The authorization properties for accessing the source code repository and to set up
        /// webhooks for notifications.
        /// </summary>
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Models.Api20190601Preview.IAuthInfo SourceControlAuthProperty { get; set; }
        /// <summary>Time in seconds that the token remains valid</summary>
        int? SourceControlAuthPropertyExpiresIn { get; set; }
        /// <summary>The refresh token used to refresh the access token.</summary>
        string SourceControlAuthPropertyRefreshToken { get; set; }
        /// <summary>The scope of the access token.</summary>
        string SourceControlAuthPropertyScope { get; set; }
        /// <summary>The access token used to access the source control provider.</summary>
        string SourceControlAuthPropertyToken { get; set; }
        /// <summary>The type of Auth token.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.TokenType? SourceControlAuthPropertyTokenType { get; set; }
        /// <summary>The type of source control service.</summary>
        Microsoft.Azure.PowerShell.Cmdlets.ContainerRegistry.Support.SourceControlType SourceControlType { get; set; }

    }
}