namespace WellBot.UseCases.Users.Common.Dtos;

/// <summary>
/// User DTO.
/// </summary>
public class UserDto
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full user name.
    /// </summary>
    public required string FullName { get; set; }
}
