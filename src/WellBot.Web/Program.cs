using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace WellBot.Web
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    [Command(Name = "wellbot")]
    internal sealed class Program
    {
        private static IHost host;

        /// <summary>
        /// Entry point method.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        public static async Task<int> Main(string[] args)
        {
            // Init host.
            host = CreateHostBuilder(args).Build();

            // Command line processing.
            var commandLineApplication = new CommandLineApplication<Program>();
            using var scope = host.Services.CreateScope();
            commandLineApplication
                .Conventions
                .UseConstructorInjection(scope.ServiceProvider)
                .UseDefaultConventions();
            return await commandLineApplication.ExecuteAsync(args);
        }

        /// <summary>
        /// This options is there to allow running the application with `--urls` parameter.
        /// https://paketo.io/docs/reference/dotnet-core-reference/#self-contained-deployment-and-framework-dependent-executables.
        /// </summary>
        [Option]
        public string Urls { get; }

        /// <summary>
        /// Create host builder.
        /// Don't change the method signature. It is used when migrating entity framework.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// Command line application execution callback.
        /// </summary>
        /// <returns>Exit code.</returns>
        public async Task<int> OnExecuteAsync()
        {
            await host.InitAsync();
            await host.RunAsync();
            return 0;
        }
    }
}
