using Microsoft.AspNetCore.Identity;

namespace WellBot.Domain.Users.Entities
{
    /// <summary>
    /// Custom application identity role.
    /// </summary>
    public class AppIdentityRole : IdentityRole<int>
    {
    }
}
