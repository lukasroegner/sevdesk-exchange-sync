
#region Using Directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Users
{
    /// <summary>
    /// Represents a list of users.
    /// </summary>
    public sealed class UserList : IEnumerable<User>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="UserList" /> instance.
        /// </summary>
        /// <param name="users">The actual list of users.</param>
        internal UserList(IEnumerable<User> users) 
        {
            this.users = users;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the actual users.
        /// </summary>
        private IEnumerable<User> users;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the skip token that can be used to get further results.
        /// </summary>
        public SkipToken SkipToken { get; internal set; }

        /// <summary>
        /// Gets the number of users to include in the results.
        /// </summary>
        public int Take { get; internal set; }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets the enumerator for the user list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<User> GetEnumerator() => this.users.GetEnumerator();

        /// <summary>
        /// Gets the enumerator for the user list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.users.GetEnumerator();

        #endregion
    }
}