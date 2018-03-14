
#region Using Directives

using System;
using SevDeskExchangeSync.Services.SevDesk.Categories;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Addresses
{
    /// <summary>
    /// Represents an address of a contact.
    /// </summary>
    public sealed class Address
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Address" /> instance.
        /// </summary>
        internal Address() { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ID of the address.
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
        /// Gets the street.
        /// </summary>
        public string Street { get; internal set; }

        /// <summary>
        /// Gets the portal code.
        /// </summary>
        public string ZipCode { get; internal set; }

        /// <summary>
        /// Gets city of the address.
        /// </summary>
        public string City { get; internal set; }

        /// <summary>
        /// Gets the country of the address.
        /// </summary>
        public Country Country { get; internal set; }

        /// <summary>
        /// Gets the name of the address.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public AddressCategory Category { get; internal set; }

        #endregion
    }
}