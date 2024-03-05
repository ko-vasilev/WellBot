using System.ComponentModel.DataAnnotations;
using MediatR;

namespace WellBot.UseCases.Users.AuthenticateUser;

/// <summary>
/// Refresh token command.
/// </summary>
public record RefreshTokenCommand : IRequest<TokenModel>
{
    /// <summary>
    /// User token.
    /// </summary>
    [Required]
    public required string Token { get; init; }
}
