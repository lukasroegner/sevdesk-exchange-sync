
#region Using Directives

using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Models
{
    /// <summary>
    /// Represents the model for a result of the API.
    /// </summary>
    internal class Result<T>
    {
        #region Public Fields

        public T objects;

        #endregion
    }
}