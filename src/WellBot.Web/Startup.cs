﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WellBot.Infrastructure.DataAccess;
using WellBot.Web.Infrastructure.Middlewares;
using WellBot.Web.Infrastructure.Settings;
using WellBot.Web.Infrastructure.Startup;

namespace WellBot.Web
{
    /// <summary>
    /// Entry point for ASP.NET Core app.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration configuration;

        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Entry point for web application.
        /// </summary>
        /// <param name="configuration">Global configuration.</param>
        /// <param name="environment">Environment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        /// <summary>
        /// Configure application services on startup.
        /// </summary>
        /// <param name="services">Services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Swagger.
            services.AddSwaggerGen(new SwaggerGenOptionsSetup().Setup);

            // CORS.
            string[] frontendOrigin = null;
            services.AddCors(new CorsOptionsSetup(
                environment.IsDevelopment(),
                frontendOrigin
                ).Setup);

            // Health check.
            services.AddHealthChecks()
                .AddSqlite(configuration["ConnectionStrings:AppDatabase"]);

            // MVC.
            services
                .AddControllers()
                .AddJsonOptions(new JsonOptionsSetup().Setup);
            services.Configure<ApiBehaviorOptions>(new ApiBehaviorOptionsSetup().Setup);

            // Identity.
            services.AddIdentity<Domain.Users.Entities.User, Domain.Users.Entities.AppIdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(new IdentityOptionsSetup().Setup);

            // JWT.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(new JwtBearerOptionsSetup(
                configuration["Jwt:SecretKey"],
                configuration["Jwt:Issuer"]).Setup
            );

            // Database.
            services.AddDbContext<AppDbContext>(
                new DbContextOptionsSetup(configuration.GetConnectionString("AppDatabase")).Setup);
            services.AddAsyncInitializer<DatabaseInitializer>();

            // Logging.
            services.AddLogging(new LoggingOptionsSetup(configuration, environment).Setup);

            // Application settings.
            services.Configure<AppSettings>(configuration.GetSection("Application"));

            // HTTP client.
            services.AddHttpClient();

            // Other dependencies.
            Infrastructure.DependencyInjection.AutoMapperModule.Register(services);
            Infrastructure.DependencyInjection.ApplicationModule.Register(services, configuration);
            Infrastructure.DependencyInjection.MediatRModule.Register(services);
            Infrastructure.DependencyInjection.SystemModule.Register(services);
        }

        /// <summary>
        /// Configure web application.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public void Configure(IApplicationBuilder app)
        {
            // Swagger
            if (!environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(new SwaggerUIOptionsSetup().Setup);
            }

            // Custom middlewares.
            app.UseMiddleware<ApiExceptionMiddleware>();

            // MVC.
            app.UseRouting();

            // CORS.
            app.UseCors(CorsOptionsSetup.CorsPolicyName);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health-check",
                    new HealthCheckOptionsSetup().Setup(
                        new HealthCheckOptions())
                );
                endpoints.Map("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
                endpoints.MapControllers();
            });
        }
    }
}
