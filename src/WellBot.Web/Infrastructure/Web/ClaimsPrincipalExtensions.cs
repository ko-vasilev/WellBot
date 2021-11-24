using System;
using System.Linq;
using System.Security.Claims;

namespace WellBot.Web.Infrastructure.Web
{
    /// <summary>
    /// Extensions for the <see cref="ClaimsPrincipal" />.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Try to get current logged user.
        /// </summary>
        /// <param name="principal">Claims principal.</param>
        /// <param name="userId">Current logged user id or -1.</param>
        /// <returns><c>True</c> if there is logged user.</returns>
        public static bool TryGetCurrentUserId(this ClaimsPrincipal principal, out int userId)
        {
            var currentUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userId = int.Parse(currentUserId);
                return true;
            }
            userId = -1;
            return false;
        }

        /// <summary>
        /// Retrieves user id from identity claims.
        /// </summary>
        /// <param name="principal">Claims principal.</param>
        /// <returns>User id.</returns>
        public static int GetCurrentUserId(this ClaimsPrincipal principal)
        {
            if (TryGetCurrentUserId(principal, out int userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Cannot get current logged user identifier.");
        }

        /// <summary>
        /// Returns roles of currently authorized user.
        /// </summary>
        public static string[] GetCurrentUserRoles(this ClaimsPrincipal principal)
        {
            var roles = principal.FindAll(ClaimTypes.Role);
            return roles.Select(r => r.Value).ToArray();
        }
    }
}
