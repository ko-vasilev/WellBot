using Riok.Mapperly.Abstractions;
using WellBot.Domain.Users;
using WellBot.UseCases.Users.Common.Dtos;
using WellBot.UseCases.Users.GetUserById;

namespace WellBot.UseCases.Users;

/// <summary>
/// Mapper for user-related models.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMapper
{
    internal partial UserDto ToUserDto(User user);

    internal partial UserDetailsDto ToUserDetailsDto(User user);

    [UserMapping(Default = true)]
    private static string MapNullableString(string? value) => value!;

    [UserMapping(Default = true)]
    private static DateTime MapNullableDateTime(DateTime? value) => value ?? default;
}
