using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WellBot.Domain.Users.Entities;

namespace WellBot.Web.Commands
{
    /// <summary>
    /// Create user command.
    /// </summary>
    [HelpOption]
    [Command("create-user", Description = "Create user.")]
    public class CreateUser
    {
        private readonly ILogger<CreateUser> logger;
        private readonly UserManager<User> userManager;

        /// <summary>
        /// Email.
        /// </summary>
        [Option("--email", Description = "User email for login.")]
        [Required]
        public string Email { get; }

        /// <summary>
        /// Password.
        /// </summary>
        [Option("--password", Description = "Password.")]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="userManager">User manager.</param>
        public CreateUser(ILogger<CreateUser> logger, UserManager<User> userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        /// <summary>
        /// Command line application execution callback.
        /// </summary>
        public async Task OnExecuteAsync()
        {
            var user = new User
            {
                Email = Email,
                UserName = Email,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, Password);
            logger.LogInformation($"User creation result: {result}.");
            if (result.Succeeded)
            {
                logger.LogInformation($"User id: {user.Id}.");
            }
        }
    }
}
