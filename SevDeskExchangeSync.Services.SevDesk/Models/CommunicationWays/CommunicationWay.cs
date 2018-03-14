
#region Using Directives

using SevDeskExchangeSync.Services.SevDesk.Models.CommunicationWayKeys;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Models.CommunicationWays
{
    /// <summary>
    /// Represents the model for a communication way of a contact.
    /// </summary>
    internal class CommunicationWay
    {
        #region Public Fields

        public string id;
        public string create;
        public string update;
        public string type;
        public string value;
        public string main;
        public CommunicationWayKey key;

        #endregion
    }
}