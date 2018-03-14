
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.CommunicationWayKinds
{
    /// <summary>
    /// Represents the base class for a communication way kinds.
    /// </summary>
    public abstract class CommunicationWayKind
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the communication way kind.
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
        /// Gets the name.
        /// </summary>
        public string Name { get; internal set; }

        #endregion
    }
}