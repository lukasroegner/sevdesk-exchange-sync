
namespace SevDeskExchangeSync.Services.SevDesk.Addresses
{
    /// <summary>
    /// Represents the options that are used to create or update addresses.
    /// </summary>
    public sealed class AddressOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the portal code.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the city of the address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the ID of the country of the address.
        /// </summary>
        public string CountryId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the category.
        /// </summary>
        public string CategoryId { get; set; }

        #endregion
    }
}