
#region Using Directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Models
{
    /// <summary>
    /// Represents the model for results of the Microsoft Graph.
    /// </summary>
    internal class ResultList<T>
    {
        #region Public Fields

        public List<T> value;
        [JsonProperty("@odata.nextLink")]
        public string odataNextLink;

        #endregion
    }
}