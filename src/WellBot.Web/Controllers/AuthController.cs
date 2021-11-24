using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using WellBot.UseCases.Users.AuthenticateUser;
using WellBot.UseCases.Users.Common.Dtos;
using WellBot.UseCases.Users.GetUserById;
using WellBot.Web.Infrastructure.Web;

namespace WellBot.Web.Controllers
{
    /// <summary>
    /// Authentication controller.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        #region Authentication

        /// <summary>
        /// Authenticate user by email and password.
        /// </summary>
        /// <param name="command">Login command.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<TokenModel> Authenticate([Required] LoginUserCommand command, CancellationToken cancellationToken)
        {
            return (await mediator.Send(command, cancellationToken)).TokenModel;
        }

        /// <summary>
        /// Get new token by refresh token.
        /// </summary>
        /// <param name="command">Refresh token command.</param>
        /// <returns>New authentication and refresh tokens.</returns>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public Task<TokenModel> RefreshToken([Required] RefreshTokenCommand command, CancellationToken cancellationToken)
            => mediator.Send(command, cancellationToken);

        /// <summary>
        /// Get current logged user info.
        /// </summary>
        /// <returns>Current logged user info.</returns>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpGet]
        [Authorize]
        public async Task<UserDto> GetMe(CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery
            {
                UserId = User.GetCurrentUserId()
            };
            return await mediator.Send(query, cancellationToken);
        }

        #endregion Authentication
    }
}
