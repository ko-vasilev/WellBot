using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WellBot.Web.Infrastructure.Startup
{
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
                ValidIssuer = issuer
            };
        }
    }
}
