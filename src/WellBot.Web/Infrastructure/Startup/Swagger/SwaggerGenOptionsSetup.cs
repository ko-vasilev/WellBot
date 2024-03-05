using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace WellBot.Web.Infrastructure.Startup.Swagger;

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
            Title = "Bot API",
            Description = "API documentation for the project."
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
        options.IncludeXmlCommentsWithRemarks(GetAssemblyLocationByType(GetType()));
        options.IncludeXmlCommentsWithRemarks(GetAssemblyLocationByType(typeof(UseCases.Common.Pagination.PageQueryFilter)));
        options.IncludeXmlCommentsWithRemarks(GetAssemblyLocationByType(typeof(UseCases.Users.AuthenticateUser.TokenModel)));
        options.IncludeXmlCommentsFromInheritDocs(includeRemarks: true);

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

        options.UseDateOnlyTimeOnlyStringConverters();
    }

    private static string GetAssemblyLocationByType(Type type) =>
        Path.Combine(AppContext.BaseDirectory, $"{type.Assembly.GetName().Name}.xml");
}
