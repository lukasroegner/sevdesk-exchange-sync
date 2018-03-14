
#region Using Directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Contacts
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
        /// Gets the number of contacts to skip.
        /// </summary>
        public int Skip { get; internal set; }

        /// <summary>
        /// Gets the number of contacts to include in the results.
        /// </summary>
        public int Take { get; internal set; }

        /// <summary>
        /// Gets the total number of contacts.
        /// </summary>
        public int Count { get; internal set; }

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