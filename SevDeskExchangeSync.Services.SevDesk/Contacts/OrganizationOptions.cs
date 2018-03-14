
namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents the options that are used to create or update an organization.
    /// </summary>
    public sealed class OrganizationOptions : ContactOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}