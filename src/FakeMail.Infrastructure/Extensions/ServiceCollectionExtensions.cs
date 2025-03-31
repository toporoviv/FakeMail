using FakeMail.Repositories.Implementations;
using FakeMail.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FakeMail.Repositories.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFakeMailRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IMailRepository, MailRepository>()
            .AddScoped<IMailMessageRepository, MailMessageRepository>();
    }
}