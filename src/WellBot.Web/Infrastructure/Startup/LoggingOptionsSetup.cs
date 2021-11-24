using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Logging setup for application.
    /// </summary>
    internal class LoggingOptionsSetup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="environment">Host environment.</param>
        public LoggingOptionsSetup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        /// <summary>
        /// Setup logging.
        /// </summary>
        /// <param name="options">Logging builder.</param>
        public void Setup(ILoggingBuilder options)
        {
            options.AddConsole();
            if (!environment.IsProduction())
            {
                options.AddDebug();
            }
            options.AddConfiguration(configuration);
        }
    }
}
