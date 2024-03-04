using System;
using WellBot.UseCases.Users.Common.Dtos;

namespace WellBot.UseCases.Users.GetUserById;

/// <summary>
/// User details.
/// </summary>
public class UserDetailsDto : UserDto
{
    /// <summary>
    /// User email.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Last login date time.
    /// </summary>
    public DateTime LastLogin { get; set; }
}
