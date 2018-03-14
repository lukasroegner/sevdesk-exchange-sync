
namespace SevDeskExchangeSync.Services.Exchange.Contacts
{
    /// <summary>
    /// Represents an address of a contact.
    /// </summary>
    public sealed class Address
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Address"/> instance.
        /// </summary>
        /// <param name="street">The street of the address.</param>
        /// <param name="zipCode">The zip code of the address.</param>
        /// <param name="city">The city of the address.</param>
        /// <param name="state">The state of the address.</param>
        /// <param name="countryOrRegion">The country or region.</param>
        public Address(string street, string zipCode, string city, string state, string countryOrRegion)
        {
            this.Street = street;
            this.ZipCode = zipCode;
            this.City = city;
            this.State = state;
            this.CountryOrRegion = countryOrRegion;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the city of the address.
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// Gets the country or region.
        /// </summary>
        public string CountryOrRegion { get; private set; }

        /// <summary>
        /// Gets the zip code of the address.
        /// </summary>
        public string ZipCode { get; private set; }

        /// <summary>
        /// Gets the state of the address.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets the street of the address.
        /// </summary>
        public string Street { get; private set; }

        #endregion
    }
}