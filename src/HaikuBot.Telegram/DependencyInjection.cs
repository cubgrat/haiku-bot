using HaikuBot.Telegram.Configuration;
using HaikuBot.Telegram.Handlers;
using HaikuBot.Telegram.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace HaikuBot.Telegram;

public static class DependencyInjection
{
    /// <summary>Регистрирует driving-адаптер Telegram.</summary>
    public static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<BotOptions>()
            .Bind(configuration.GetSection(BotOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Token), "Не задан Bot:Token (BOT__TOKEN).");

        services.AddHttpClient("telegram")
            .AddTypedClient<ITelegramBotClient>((http, sp) =>
            {
                var token = sp.GetRequiredService<IOptions<BotOptions>>().Value.Token;
                return new TelegramBotClient(token, http);
            });

        services.AddSingleton<InFlightTracker>();
        services.AddScoped<UpdateHandler>();
        services.AddHostedService<TelegramBotWorker>();
        return services;
    }
}
