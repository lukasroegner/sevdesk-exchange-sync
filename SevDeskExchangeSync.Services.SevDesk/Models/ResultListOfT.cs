
#region Using Directives

using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Models
{
    /// <summary>
    /// Represents the model for results of the API.
    /// </summary>
    internal class ResultList<T>
    {
        #region Public Fields

        public int total;
        public List<T> objects;

        #endregion
    }
}