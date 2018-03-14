
namespace SevDeskExchangeSync.Services.Exchange.Contacts
{
    /// <summary>
    /// Represents an email address of a contact.
    /// </summary>
    public sealed class EmailAddress
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="EmailAddress"/> instance.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="address">The email address.</param>
        public EmailAddress(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string Address { get; private set; }

        #endregion
    }
}