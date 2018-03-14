
namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents a organization in the sevDesk web application.
    /// </summary>
    public sealed class Organization : Contact
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Organization" /> instance.
        /// </summary>
        internal Organization() { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        public string Name { get; internal set; }

        #endregion
    }
}