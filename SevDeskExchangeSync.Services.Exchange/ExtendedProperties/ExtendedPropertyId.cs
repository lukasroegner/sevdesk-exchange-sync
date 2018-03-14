
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.ExtendedProperties
{
    /// <summary>
    /// Represents an extended property ID.
    /// </summary>
    public sealed class ExtendedPropertyId
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ExtendedPropertyId"/> instance.
        /// </summary>
        /// <param name="dataType">The data type of the ID.</param>
        /// <param name="namespaceId">The GUID of the ID's namespace.</param>
        /// <param name="name">The name of the ID.</param>
        public ExtendedPropertyId(ExtendedPropertyDataType dataType, Guid namespaceId, string name)
        {
            this.DataType = dataType;
            this.NamespaceId = namespaceId;
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new <see cref="ExtendedPropertyId"/> instance.
        /// </summary>
        /// <param name="extendedPropertyId">The string representation of the ID.</param>
        public ExtendedPropertyId(string extendedPropertyId)
        {
            string[] parts = extendedPropertyId.Split(' ');
            this.DataType = (ExtendedPropertyDataType)Enum.Parse(typeof(ExtendedPropertyDataType), parts[0]);
            this.NamespaceId = new Guid(parts[1]);
            this.Name = parts[3];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the ID.
        /// </summary>
        public ExtendedPropertyDataType DataType { get; private set; }

        /// <summary>
        /// Gets the GUID of the ID's namespace.
        /// </summary>
        public Guid NamespaceId { get; private set; }

        /// <summary>
        /// Gets the name of the ID.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="ExtendedPropertyId"/>.
        /// </summary>
        /// <returns>Returns a <see cref="String"/> that represents the current <see cref="ExtendedPropertyId"/>.</returns>
        public override string ToString() => $"{this.DataType} {{{this.NamespaceId.ToString()}}} Name {this.Name}";

        #endregion
    }
}