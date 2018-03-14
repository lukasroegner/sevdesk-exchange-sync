
namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents the base class for options that are used to create or update contacts.
    /// </summary>
    public abstract class ContactOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the customer number.
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID of the category.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the description of the contact.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}