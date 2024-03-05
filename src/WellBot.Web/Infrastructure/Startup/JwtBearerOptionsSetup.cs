using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WellBot.Web.Infrastructure.Startup;

/// <summary>
/// JWT bearer options setup.
/// </summary>
internal class JwtBearerOptionsSetup
{
    private readonly string secretKey;
    private readonly string issuer;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="secretKey">JWT secret key.</param>
    /// <param name="issuer">JWT issuer.</param>
    public JwtBearerOptionsSetup(string secretKey, string issuer)
    {
        this.secretKey = secretKey;
        this.issuer = issuer;
    }

    /// <summary>
    /// Setup JWT options.
    /// </summary>
    /// <param name="options">The options.</param>
    public void Setup(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidIssuer = issuer,
            ValidateLifetime = true,
            LifetimeValidator = ValidateTokenLifetime,
        };
    }

    private static bool ValidateTokenLifetime(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        var clonedParameters = validationParameters.Clone();
        // Reset the lifetime validator to avoid the infinite loop.
        clonedParameters.LifetimeValidator = null;
        try
        {
            Validators.ValidateLifetime(notBefore, expires, securityToken, clonedParameters);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}
