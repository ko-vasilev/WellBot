using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Automatically adds information about authorization requirements for API endpoints.
    /// </summary>
    internal sealed class SwaggerSecurityRequirementsOperationFilter : IOperationFilter
    {
        private static readonly string UnauthorizedCode = StatusCodes.Status401Unauthorized.ToString();
        private static readonly string ForbiddenCode = StatusCodes.Status403Forbidden.ToString();

        private static readonly OpenApiSecurityRequirement BearerSecurityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            },
        };

        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!DoesActionRequireAuthorization(context.MethodInfo))
            {
                return;
            }

            operation.Responses.Add(UnauthorizedCode, new OpenApiResponse { Description = "Unauthorized" });
            operation.Security.Add(BearerSecurityRequirement);

            TryAddForbiddenResponse(operation, context);
        }

        private static bool DoesActionRequireAuthorization(MethodInfo methodInfo)
        {
            if (methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return false;
            }

            if (methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any() ||
                methodInfo.ReflectedType.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any())
            {
                return true;
            }

            return false;
        }

        private void TryAddForbiddenResponse(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo.ReflectedType
                .GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .ToList();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            if (!operation.Responses.ContainsKey(ForbiddenCode))
            {
                operation.Responses.Add(ForbiddenCode, new OpenApiResponse
                {
                    Description = "Forbidden"
                });
            }

            var roleList = authorizeAttributes
                .Select(attr => attr.Roles)
                .Where(roles => !string.IsNullOrEmpty(roles))
                .SelectMany(roles => roles.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            var permissionList = authorizeAttributes
                .Select(attr => attr.Policy)
                .Where(policy => !string.IsNullOrEmpty(policy))
                .Distinct()
                .ToList();

            if (!roleList.Any() && !permissionList.Any())
            {
                return;
            }

            var forbiddenResponse = operation.Responses["403"];

            // Update response description with list of required roles.
            var sb = new StringBuilder(200);
            // Keep the original description.
            sb.AppendLine(forbiddenResponse.Description);

            if (roleList.Any())
            {
                sb.Append("<br />Roles Required: ");
                sb.AppendJoin(", ", roleList.Select(role => $"<code>{role}</code>"));
            }
            if (permissionList.Any())
            {
                sb.Append("<br />Permissions Required: ");
                sb.AppendJoin(", ", permissionList.Select(p => $"<code>{p}</code>"));
            }

            forbiddenResponse.Description = sb.ToString();
        }
    }
}
