// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.
namespace Microsoft.Azure.Management.RecoveryServices.SiteRecovery
{
    using Microsoft.Rest.Azure;
    using Models;

    /// <summary>
    /// Extension methods for ReplicationVaultHealthOperations
    /// </summary>
    public static partial class ReplicationVaultHealthOperationsExtensions
    {
        /// <summary>
        /// Gets the health details of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        public static VaultHealthDetails Get(this IReplicationVaultHealthOperations operations)
        {
                return ((IReplicationVaultHealthOperations)operations).GetAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the health details of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async System.Threading.Tasks.Task<VaultHealthDetails> GetAsync(this IReplicationVaultHealthOperations operations, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            using (var _result = await operations.GetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
        /// <summary>
        /// Refreshes health summary of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        public static VaultHealthDetails Refresh(this IReplicationVaultHealthOperations operations)
        {
                return ((IReplicationVaultHealthOperations)operations).RefreshAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Refreshes health summary of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async System.Threading.Tasks.Task<VaultHealthDetails> RefreshAsync(this IReplicationVaultHealthOperations operations, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            using (var _result = await operations.RefreshWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
        /// <summary>
        /// Refreshes health summary of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        public static VaultHealthDetails BeginRefresh(this IReplicationVaultHealthOperations operations)
        {
                return ((IReplicationVaultHealthOperations)operations).BeginRefreshAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Refreshes health summary of the vault.
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async System.Threading.Tasks.Task<VaultHealthDetails> BeginRefreshAsync(this IReplicationVaultHealthOperations operations, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            using (var _result = await operations.BeginRefreshWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}
