
namespace SevDeskExchangeSync.Services.Exchange.ExtendedProperties
{
    /// <summary>
    /// Represents an extended property.
    /// </summary>
    public sealed class ExtendedProperty
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ExtendedProperty"/> instance.
        /// </summary>
        /// <param name="id">The extended property ID.</param>
        /// <param name="value">The value of the property.</param>
        public ExtendedProperty(ExtendedPropertyId id, string value)
        {
            this.Id = id;
            this.Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ID of the extended property.
        /// </summary>
        public ExtendedPropertyId Id { get; private set; }

        /// <summary>
        /// Gets the value of the extended property.
        /// </summary>
        public string Value { get; private set; }

        #endregion
    }
}