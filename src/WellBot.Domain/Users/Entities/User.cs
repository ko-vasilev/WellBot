using System;
using Microsoft.AspNetCore.Identity;

namespace WellBot.Domain.Users.Entities
{
    /// <summary>
    /// Custom application user entity.
    /// </summary>
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Admin user id.
        /// </summary>
        public const int AdminUserId = 1;

        /// <summary>
        /// The date when user last logged in.
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Last token reset date. Before the date all generate login tokens are considered
        /// not valid. Must be in UTC format.
        /// </summary>
        public DateTime LastTokenResetAt { get; set; } = DateTime.UtcNow;
    }
}
