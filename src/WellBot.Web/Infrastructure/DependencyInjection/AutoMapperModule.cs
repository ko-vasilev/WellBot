using Microsoft.Extensions.DependencyInjection;
using WellBot.UseCases.Users.AuthenticateUser;

namespace WellBot.Web.Infrastructure.DependencyInjection;

/// <summary>
/// Register AutoMapper dependencies.
/// </summary>
public class AutoMapperModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(TokenModel).Assembly);
    }
}
