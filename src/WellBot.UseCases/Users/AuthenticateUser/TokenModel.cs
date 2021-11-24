namespace WellBot.UseCases.Users.AuthenticateUser
{
    /// <summary>
    /// API generated token model.
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration in seconds.
        /// </summary>
        public long ExpiresIn { get; set; }
    }
}
