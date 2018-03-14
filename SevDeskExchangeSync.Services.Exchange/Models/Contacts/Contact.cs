
#region Using Directives

using System.Collections.Generic;

#endregion

namespace SevDeskExchangeSync.Services.Exchange.Models.Contacts
{
    /// <summary>
    /// Represents the model for the contact endpoint.
    /// </summary>
    internal class Contact
    {
        #region Public Fields

        public string id;
        public string createdDateTime;
        public string lastModifiedDateTime;
        public string changeKey;
        public string birthday;
        public string fileAs;
        public string displayName;
        public string givenName;
        public string surname;
        public string title;
        public string jobTitle;
        public string companyName;
        public string businessHomePage;
        public List<string> homePhones;
        public string mobilePhone;
        public List<string> businessPhones;
        public string personalNotes;
        public List<EmailAddress> emailAddresses;
        public Address homeAddress;
        public Address businessAddress;
        public Address otherAddress;
        public List<SingleValueExtendedProperty> singleValueExtendedProperties;

        #endregion
    }
}