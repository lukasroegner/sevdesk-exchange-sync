
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Addresses
{
    /// <summary>
    /// Represents the country of an address.
    /// </summary>
    public sealed class Country
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Country" /> instance.
        /// </summary>
        internal Country() { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ID of the country.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Gets the name of the country.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the name of the country in English.
        /// </summary>
        public string EnglishName { get; internal set; }

        /// <summary>
        /// Gets the priority of the country.
        /// </summary>
        public Nullable<int> Priority { get; internal set; }

        #endregion
    }
}