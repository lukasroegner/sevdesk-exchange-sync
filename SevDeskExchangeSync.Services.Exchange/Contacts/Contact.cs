
#region Using Directives

using System;
using System.Collections.Generic;
using SevDeskExchangeSync.Services.Exchange.ExtendedProperties;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Contacts
{
    /// <summary>
    /// Represents a contact.
    /// </summary>
    public sealed class Contact
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the contact.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the version of the contact.
        /// </summary>
        public string ChangeKey { get; internal set; }

        /// <summary>
        /// Gets the creation date time.
        /// </summary>
        public DateTime CreationDateTime { get; internal set; }

        /// <summary>
        /// Gets the last modified date time.
        /// </summary>
        public DateTime LastModifiedDateTime { get; internal set; }

        /// <summary>
        /// Gets the birthday.
        /// </summary>
        public Nullable<DateTime> Birthday { get; internal set; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName { get; internal set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName { get; internal set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Gets the job title.
        /// </summary>
        public string JobTitle { get; internal set; }

        /// <summary>
        /// Gets the company name.
        /// </summary>
        public string CompanyName { get; internal set; }

        /// <summary>
        /// Gets the business website.
        /// </summary>
        public string BusinessWebsite { get; internal set; }

        /// <summary>
        /// Gets a list of home phone numbers.
        /// </summary>
        public IEnumerable<string> HomePhoneNumbers { get; internal set; }

        /// <summary>
        /// Gets the mobile phone number.
        /// </summary>
        public string MobilePhoneNumber { get; internal set; }

        /// <summary>
        /// Gets a list of business phone numbers.
        /// </summary>
        public IEnumerable<string> BusinessPhoneNumbers { get; internal set; }

        /// <summary>
        /// Gets the personal notes.
        /// </summary>
        public string PersonalNotes { get; internal set; }

        /// <summary>
        /// Gets a list of email addresses.
        /// </summary>
        public IEnumerable<EmailAddress> EmailAddresses { get; internal set; }

        /// <summary>
        /// Gets the home address.
        /// </summary>
        public Address HomeAddress { get; internal set; }

        /// <summary>
        /// Gets the business address.
        /// </summary>
        public Address BusinessAddress { get; internal set; }

        /// <summary>
        /// Gets the other address.
        /// </summary>
        public Address OtherAddress { get; internal set; }

        /// <summary>
        /// Gets a list of extended properties
        /// </summary>
        public IEnumerable<ExtendedProperty> ExtendedProperties { get; internal set; }

        #endregion
    }
}