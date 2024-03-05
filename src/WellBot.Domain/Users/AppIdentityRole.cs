using Microsoft.AspNetCore.Identity;

namespace WellBot.Domain.Users;

/// <summary>
/// Custom application identity role.
/// </summary>
public class AppIdentityRole : IdentityRole<int>
{
}
