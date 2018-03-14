
#region Using Directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Contacts
{
    /// <summary>
    /// Represents a list of contacts.
    /// </summary>
    public sealed class ContactList : IEnumerable<Contact>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ContactList" /> instance.
        /// </summary>
        /// <param name="contacts">The actual list of contacts.</param>
        internal ContactList(IEnumerable<Contact> contacts) 
        {
            this.contacts = contacts;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the actual contacts.
        /// </summary>
        private IEnumerable<Contact> contacts;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the skip token that can be used to get further results.
        /// </summary>
        public SkipToken SkipToken { get; internal set; }

        /// <summary>
        /// Gets the number of contacts to include in the results.
        /// </summary>
        public int Take { get; internal set; }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets the enumerator for the contact list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<Contact> GetEnumerator() => this.contacts.GetEnumerator();

        /// <summary>
        /// Gets the enumerator for the contact list.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.contacts.GetEnumerator();

        #endregion
    }
}