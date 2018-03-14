
#region Using Directives

using System;
using System.Collections.Generic;
using SevDeskExchangeSync.Services.Exchange.ExtendedProperties;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Contacts
{
    /// <summary>
    /// Represents the options that are used to create or update a contact.
    /// </summary>
    public sealed class ContactOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the birthday.
        /// </summary>
        public Nullable<DateTime> Birthday { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the business website.
        /// </summary>
        public string BusinessWebsite { get; set; }

        /// <summary>
        /// Gets or sets a list of home phone numbers.
        /// </summary>
        public IEnumerable<string> HomePhoneNumbers { get; set; }

        /// <summary>
        /// Gets or sets the mobile phone number.
        /// </summary>
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a list of business phone numbers.
        /// </summary>
        public IEnumerable<string> BusinessPhoneNumbers { get; set; }

        /// <summary>
        /// Gets or sets the personal notes.
        /// </summary>
        public string PersonalNotes { get; set; }

        /// <summary>
        /// Gets or sets the job title.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets a list of email addresses.
        /// </summary>
        public IEnumerable<EmailAddress> EmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets the home address.
        /// </summary>
        public Address HomeAddress { get; set; }

        /// <summary>
        /// Gets or sets the business address.
        /// </summary>
        public Address BusinessAddress { get; set; }

        /// <summary>
        /// Gets or sets the other address.
        /// </summary>
        public Address OtherAddress { get; set; }

        /// <summary>
        /// Gets or sets a list of extended properties
        /// </summary>
        public IEnumerable<ExtendedProperty> ExtendedProperties { get; set; }

        #endregion
    }
}