using HaikuBot.Application.Configuration;
using HaikuBot.Application.Ports.Inbound;
using HaikuBot.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HaikuBot.Application;

public static class DependencyInjection
{
    /// <summary>Регистрирует use-cases приложения и их настройки.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PipelineOptions>()
            .Bind(configuration.GetSection(PipelineOptions.SectionName));

        services.AddScoped<IHaikuService, HaikuService>();
        return services;
    }
}
