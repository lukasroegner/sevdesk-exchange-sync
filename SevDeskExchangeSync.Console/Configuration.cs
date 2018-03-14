
#region Using Directives

using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Console
{
    /// <summary>
    /// Represents the configuration of the synchronization process.
    /// </summary>
    public sealed class Configuration
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the API token for sevDesk.
        /// </summary>
        public string SevDeskApiToken { get; set; }

        /// <summary>
        /// Gets or sets a list of IDs of the contact categories that should be synchronized. Is <c>null</c> is all categories should be synchronized.
        /// </summary>
        public List<string> SevDeskContactCategories { get; set; }

        /// <summary>
        /// Gets or sets the client ID of the Azure application.
        /// </summary>
        public string AzureClientId { get; set; }

        /// <summary>
        /// Gets or sets the tenant of the Azure Active Directory.
        /// </summary>
        public string AzureActiveDirectoryTenant { get; set; }

        /// <summary>
        /// Gets or sets the client secret of the Azure application.
        /// </summary>
        public string AzureClientSecret { get; set; }

        /// <summary>
        /// Gets or sets a list of IDs of the Azure Active Directory users that should participate in the synchronization. Is <c>null</c> is all users should be synchronized.
        /// </summary>
        public List<string> AzureActiveDirectoryUsers { get; set; }

        /// <summary>
        /// Gets or sets the file name for the contact sync states.
        /// </summary>
        public string ContactSyncStatesFileName { get; set; }

        #endregion
    }
}