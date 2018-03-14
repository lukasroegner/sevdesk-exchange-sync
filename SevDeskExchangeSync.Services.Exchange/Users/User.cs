
namespace SevDeskExchangeSync.Services.Exchange.Users
{
    /// <summary>
    /// Represents a user of the Azure Active Directory.
    /// </summary>
    public sealed class User
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the user principal name.
        /// </summary>
        public string UserPrincipalName { get; internal set; }

        #endregion
    }
}