
#region Using Directives

using System;
using SevDeskExchangeSync.Services.SevDesk.CommunicationWayKinds;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.CommunicationWays
{
    /// <summary>
    /// Represents the base class for a communication way of a contact.
    /// </summary>
    public abstract class CommunicationWay
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the communication way.
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
        /// Gets the value.
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// Gets a value that determines whether this is the primary communication way.
        /// </summary>
        public bool IsPrimary { get; internal set; }

        /// <summary>
        /// Gets the kind of communication way.
        /// </summary>
        public CommunicationWayKind Kind { get; internal set; }

        #endregion
    }
}