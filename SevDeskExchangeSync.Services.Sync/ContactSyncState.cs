
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.Sync
{
    /// <summary>
    /// Represents the synchronization state of a contact.
    /// </summary>
    public class ContactSyncState
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the sevDesk contact ID.
        /// </summary>
        public string SevDeskContactId { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string ExchangeUserId { get; set; }

        /// <summary>
        /// Gets or sets the sevDesk contact update date time.
        /// </summary>
        public DateTime SevDeskContactUpdateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Exchange contact ID.
        /// </summary>
        public string ExchangeContactId { get; set; }

        /// <summary>
        /// Gets or sets the Exchange contact last modified date time.
        /// </summary>
        public DateTime ExchangeContactLastModifiedDateTime { get; set; }

        #endregion
    }
}
