
#region Using Directives

using System;
using System.Linq;

#endregion

namespace SevDeskExchangeSync.Services.Exchange
{
    /// <summary>
    /// Represents a user of the Azure Active Directory.
    /// </summary>
    public sealed class SkipToken
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="SkipToken"/> instance.
        /// </summary>
        /// <param name="uri">The uri that is parsed.</param>
        internal SkipToken(string uri)
        {
            this.Token = new Uri(uri).Query.Split('&').Where(parameter => parameter.Split('=')[0] == "$skiptoken").Select(parameter => parameter.Split('=')[1]).FirstOrDefault();   
            this.Skip = new Uri(uri).Query.Split('&').Where(parameter => parameter.Split('=')[0] == "$skip").Select(parameter => int.Parse(parameter.Split('=')[1])).FirstOrDefault();  
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the token.
        /// </summary>
        internal string Token { get; private set; }

        /// <summary>
        /// Gets the skip parameter.
        /// </summary>
        internal Nullable<int> Skip { get; private set; }

        #endregion
    }
}