
#region Using Directives

using SevDeskExchangeSync.Services.SevDesk.Models.Categories;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Models.Addresses
{
    /// <summary>
    /// Represents the model for an address of a contact.
    /// </summary>
    internal class Address
    {
        #region Public Fields

        public string id;
        public string create;
        public string update;
        public string street;
        public string zip;
        public string city;
        public Country country;
        public string name;
        public Category category;

        #endregion
    }
}