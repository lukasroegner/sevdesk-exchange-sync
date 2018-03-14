
namespace SevDeskExchangeSync.Services.Exchange.Models
{
    /// <summary>
    /// Represents the model for the token response.
    /// </summary>
    internal class Token
    {
        #region Public Fields

        public string token_type;
        public int expires_in;
        public int ext_expires_in;
        public string access_token;

        #endregion
    }
}