
namespace SevDeskExchangeSync.Services.SevDesk.Users
{
    /// <summary>
    /// Represents a user of the sevDesk web application.
    /// </summary>
    public sealed class User
    {
        #region Public Properties

        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        public string UserName { get; internal set; }

        #endregion
    }
}