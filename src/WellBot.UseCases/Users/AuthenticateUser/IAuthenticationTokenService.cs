using System.Security.Claims;

namespace WellBot.UseCases.Users.AuthenticateUser;

/// <summary>
/// Methods to help generate and parse authentication token.
/// </summary>
public interface IAuthenticationTokenService
{
    /// <summary>
    /// Generate access token.
    /// </summary>
    /// <param name="claims">User claims.</param>
    /// <param name="expirationTime">Token expiration time.</param>
    /// <returns>Token.</returns>
    string GenerateToken(IEnumerable<Claim> claims, TimeSpan expirationTime);

    /// <summary>
    /// Get token claims.
    /// </summary>
    /// <param name="token">User token.</param>
    /// <returns>User claims.</returns>
    IEnumerable<Claim> GetTokenClaims(string token);
}
