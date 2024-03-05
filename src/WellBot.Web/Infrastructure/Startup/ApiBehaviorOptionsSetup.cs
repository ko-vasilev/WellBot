using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using WellBot.Web.Infrastructure.Middlewares;
using Saritasa.Tools.Domain.Exceptions;

namespace WellBot.Web.Infrastructure.Startup;

/// <summary>
/// API behavior setup. In this behavior we override default 400 errors handler to
/// use another "errors" field of <see cref="ValidationProblemDetails" />.
/// </summary>
internal class ApiBehaviorOptionsSetup
{
    private readonly string? code;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="code">Optional code to include into response.</param>
    public ApiBehaviorOptionsSetup(string? code = null)
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
                var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>();
                problemDetails.Extensions[ApiExceptionMiddleware.ErrorsKey] = GetProblemFields(context.ModelState, jsonOptions.Value);
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        };
    }

    private static IEnumerable<ProblemFieldDto> GetProblemFields(ModelStateDictionary modelState, JsonOptions jsonOptions)
    {
        foreach (var modelStateEntry in modelState)
        {
            var fieldName = ApiExceptionMiddleware.FormatPropertyPath(modelStateEntry.Key, jsonOptions.JsonSerializerOptions);
            foreach (var error in modelStateEntry.Value.Errors.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)))
            {
                yield return new ProblemFieldDto(fieldName, error.ErrorMessage);
            }
        }
    }
}
