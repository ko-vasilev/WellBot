using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Swagger generation options.
    /// </summary>
    internal class SwaggerGenOptionsSetup
    {
        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="options">Swagger generation options.</param>
        public void Setup(SwaggerGenOptions options)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = fileVersionInfo.ProductVersion,
                // TODO:
                Title = "Swagger Setup Example",
                Description = "API documentation for the project.",
                Contact = new OpenApiContact
                {
                    Name = "Saritasa",
                    Email = "team@saritasa.com"
                }
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insert JWT token to the field.",
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "bearer",
                Type = SecuritySchemeType.Http
            });
            // TODO: Add your assemblies here.
            options.IncludeXmlComments(GetAssemblyLocationByType(GetType()));
            options.IncludeXmlComments(GetAssemblyLocationByType(typeof(UseCases.Common.Pagination.PageQueryFilter)));
            options.IncludeXmlComments(GetAssemblyLocationByType(typeof(UseCases.Users.AuthenticateUser.TokenModel)));

            // Our custom filters.
            options.SchemaFilter<SwaggerExampleSetterSchemaFilter>();
            options.SchemaFilter<SwaggerEnumDescriptionSchemaOperationFilter>();
            options.OperationFilter<SwaggerEnumDescriptionSchemaOperationFilter>();
            options.OperationFilter<SwaggerSecurityRequirementsOperationFilter>();

            // Group by ApiExplorerSettings.GroupName name.
            options.TagActionsBy(apiDescription => new[]
            {
                apiDescription.GroupName
            });
            options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));

            options.CustomOperationIds(a =>
                a.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
                    ? string.Concat(controllerActionDescriptor.ControllerName, "/", controllerActionDescriptor.ActionName)
                    : string.Empty);
        }

        private static string GetAssemblyLocationByType(Type type) =>
            System.IO.Path.Combine(AppContext.BaseDirectory, $"{type.Assembly.GetName().Name}.xml");
    }
}
