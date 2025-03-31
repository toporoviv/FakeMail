using FakeMail.Services.Implementations;
using FakeMail.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FakeMail.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFakeMailServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AccountService>()
            .AddScoped<IRegistrationService, AccountService>()
            .AddScoped<IMailService, MailService>();
    }
}