using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WellBot.Web.Infrastructure.Middlewares;
using Saritasa.Tools.Domain.Exceptions;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// API behavior setup. In this behavior we override default 400 errors handler to
    /// use another "errors" field of <see cref="ValidationProblemDetails" />.
    /// </summary>
    internal class ApiBehaviorOptionsSetup
    {
        private readonly string code;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">Optional code to include into response.</param>
        public ApiBehaviorOptionsSetup(string code = null)
        {
            this.code = code;
        }

        /// <summary>
        /// Setup API behavior.
        /// </summary>
        /// <param name="options">API behavior options.</param>
        public void Setup(ApiBehaviorOptions options)
        {
            options.SuppressMapClientErrors = true;
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = "Please refer to the errors for additional details.",
                    Type = nameof(ValidationException),
                    Status = StatusCodes.Status400BadRequest
                };

                // Fill code.
                if (!string.IsNullOrEmpty(code))
                {
                    problemDetails.Extensions[ApiExceptionMiddleware.CodeKey] = code;
                }

                // Fill errors.
                if (context.ModelState.Any())
                {
                    problemDetails.Extensions[ApiExceptionMiddleware.ErrorsKey] =
                        context.ModelState.Select(ms => new ProblemFieldDto(
                            ToLowerCamelCase(ms.Key),
                            ms.Value.Errors.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage)));
                }
                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            };
        }

        // We assume that target string uses lowerCamelCase.
        private static string ToLowerCamelCase(string target)
        {
            if (target.Length < 1)
            {
                return target;
            }
            var ch = target[0];
            return char.IsLower(ch) ? target : string.Concat(char.ToLower(ch), target.Substring(1));
        }
    }
}
