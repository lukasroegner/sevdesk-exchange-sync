
namespace SevDeskExchangeSync.Services.SevDesk.CommunicationWays
{
    /// <summary>
    /// Represents the base class for options that are used to create or update communication ways.
    /// </summary>
    public abstract class CommunicationWayOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the ID of the kind of communication way.
        /// </summary>
        public string KindId { get; set; }

        #endregion
    }
}