
#region Using Directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Categories
{
    /// <summary>
    /// Represents a list of categories.
    /// </summary>
    public sealed class CategoryList : IEnumerable<Category>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="CategoryList" /> instance.
        /// </summary>
        /// <param name="categories">The actual list of categories.</param>
        internal CategoryList(IEnumerable<Category> categories) 
        {
            this.categories = categories;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the actual categories.
        /// </summary>
        private IEnumerable<Category> categories;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of categories to skip.
        /// </summary>
        public int Skip { get; internal set; }

        /// <summary>
        /// Gets the number of categories to include in the results.
        /// </summary>
        public int Take { get; internal set; }

        /// <summary>
        /// Gets the total number of categories.
        /// </summary>
        public int Count { get; internal set; }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets the enumerator for the category list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<Category> GetEnumerator() => this.categories.GetEnumerator();

        /// <summary>
        /// Gets the enumerator for the category list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.categories.GetEnumerator();

        #endregion
    }
}