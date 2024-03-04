using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WellBot.Infrastructure.Abstractions.Interfaces;
using Saritasa.Tools.Domain.Exceptions;
using WellBot.Domain.Users;

namespace WellBot.UseCases.Users.AuthenticateUser
{
    /// <summary>
    /// Handler for <see cref="RefreshTokenCommand" />.
    /// </summary>
    internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenModel>
    {
        private readonly IAuthenticationTokenService tokenService;
        private readonly SignInManager<User> signInManager;
        private readonly IAppDbContext appDbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tokenService">Token service.</param>
        /// <param name="signInManager">Sign in manager.</param>
        /// <param name="appDbContext">Database context.</param>
        public RefreshTokenCommandHandler(
            IAuthenticationTokenService tokenService,
            SignInManager<User> signInManager,
            IAppDbContext appDbContext)
        {
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task<TokenModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Get user.
            var userId = GetTokenUserId(request.Token);
            var user = await signInManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new DomainException($"User with identifier {userId} not found.");
            }

            // Validate token.
            var tokenCreationDate = GetTokenCreationDate(request.Token);
            if (tokenCreationDate + AuthenticationConstants.RefreshTokenExpire <= DateTime.UtcNow ||
                tokenCreationDate < user.LastTokenResetAt)
            {
                throw new DomainException("Token has been expired.");
            }

            var principal = await signInManager.CreateUserPrincipalAsync(user);
            return TokenModelGenerator.Generate(tokenService, principal.Claims);
        }

        private DateTime GetTokenCreationDate(string token)
        {
            var tokenClaims = GetTokenClaims(token);
            var iatClaim = tokenClaims.FirstOrDefault(c => c.Type == AuthenticationConstants.IatClaimType);
            if (iatClaim == null)
            {
                throw new DomainException("Iat claim cannot be found. Invalid token.");
            }
            return new DateTime(long.Parse(iatClaim.Value), DateTimeKind.Utc);
        }

        private string GetTokenUserId(string token)
        {
            var tokenClaims = GetTokenClaims(token);
            var userIdClaim = tokenClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new DomainException(
                    "User identifier claim cannot be found. Invalid token.");
            }
            return userIdClaim.Value;
        }

        private IEnumerable<Claim> GetTokenClaims(string token)
        {
            try
            {
                return tokenService.GetTokenClaims(token);
            }
            catch (Exception)
            {
                throw new DomainException("Invalid token.");
            }
        }
    }
}
