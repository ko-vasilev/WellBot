using Microsoft.Extensions.DependencyInjection;
using WellBot.UseCases.Chats;
using WellBot.UseCases.Users;

namespace WellBot.Web.Infrastructure.DependencyInjection;

/// <summary>
/// Registers generated mappers.
/// </summary>
internal static class MapperModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<ChatMapper>();
        services.AddSingleton<UserMapper>();
    }
}
