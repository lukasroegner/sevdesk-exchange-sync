
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Categories
{
    /// <summary>
    /// Represents a category of the sevDesk web application.
    /// </summary>
    public abstract class Category
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the category.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the creation date time.
        /// </summary>
        public Nullable<DateTime> CreationDateTime { get; internal set; }

        /// <summary>
        /// Gets the update date time.
        /// </summary>
        public Nullable<DateTime> UpdateDateTime { get; internal set; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the priority of the category.
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Gets the category code.
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Gets the color of the category as HTML hexadecimal color code.
        /// </summary>
        public string Color { get; internal set; }

        #endregion
    }
}