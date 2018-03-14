
#region Using Directives

using System;
using System.Collections.Generic;
using SevDeskExchangeSync.Services.SevDesk.Categories;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents a contact in the sevDesk web application.
    /// </summary>
    public abstract class Contact
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the contact.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the creation date time.
        /// </summary>
        public DateTime CreationDateTime { get; internal set; }

        /// <summary>
        /// Gets the update date time.
        /// </summary>
        public DateTime UpdateDateTime { get; internal set; }

        /// <summary>
        /// Gets the customer number.
        /// </summary>
        public string CustomerNumber { get; internal set; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public ContactCategory Category { get; internal set; }

        /// <summary>
        /// Gets the description of the contact.
        /// </summary>
        public string Description { get; internal set; }

        #endregion
    }
}