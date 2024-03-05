using MediatR;
using WellBot.UseCases.Users.AuthenticateUser;
using WellBot.Web.Infrastructure.Telegram;

namespace WellBot.Web.Infrastructure.DependencyInjection;

/// <summary>
/// Register Mediator as dependency.
/// </summary>
internal static class MediatRModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TelegramChatSetterPipelineBehavior<,>));
    }
}
