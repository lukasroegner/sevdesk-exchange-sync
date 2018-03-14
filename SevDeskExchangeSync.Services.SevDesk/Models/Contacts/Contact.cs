
#region Using Directives

using System.Collections.Generic;
using SevDeskExchangeSync.Services.SevDesk.Models.Addresses;
using SevDeskExchangeSync.Services.SevDesk.Models.Categories;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Models.Contacts
{
    /// <summary>
    /// Represents the model for the contacts endpoint.
    /// </summary>
    internal class Contact
    {
        #region Public Fields

        public string id;
        public string create;
        public string update;
        public string name;
        public string customerNumber;
        public string surename;
        public string familyname;
        public string titel;
        public Category category;
        public string description;
        public string academicTitle;
        public string gender;
        public string birthday;
        public Contact parent;
        public List<Address> addresses;

        #endregion
    }
}