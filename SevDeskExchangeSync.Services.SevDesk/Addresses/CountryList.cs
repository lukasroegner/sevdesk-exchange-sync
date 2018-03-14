
#region Using Directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Addresses
{
    /// <summary>
    /// Represents a list of countries.
    /// </summary>
    public sealed class CountryList : IEnumerable<Country>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="CountryList" /> instance.
        /// </summary>
        /// <param name="countries">The actual list of countries.</param>
        internal CountryList(IEnumerable<Country> countries) 
        {
            this.countries = countries;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the actual countries.
        /// </summary>
        private IEnumerable<Country> countries;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of countries to skip.
        /// </summary>
        public int Skip { get; internal set; }

        /// <summary>
        /// Gets the number of countries to include in the results.
        /// </summary>
        public int Take { get; internal set; }

        /// <summary>
        /// Gets the total number of countries.
        /// </summary>
        public int Count { get; internal set; }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets the enumerator for the countries list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<Country> GetEnumerator() => this.countries.GetEnumerator();

        /// <summary>
        /// Gets the enumerator for the countries list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.countries.GetEnumerator();

        #endregion
    }
}