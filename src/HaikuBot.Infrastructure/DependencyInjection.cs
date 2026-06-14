using System.Net.Http.Headers;
using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Infrastructure.Llm.ApiFreeLlm;
using HaikuBot.Infrastructure.Llm.Configuration;
using HaikuBot.Infrastructure.Llm.Mock;
using HaikuBot.Infrastructure.Llm.OpenAi;
using HaikuBot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HaikuBot.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Регистрирует driven-адаптеры: хранилище (PostgreSQL) и нейросети.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddLlm(services, configuration);
        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Postgres.");

        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));
        services.AddScoped<IHaikuRepository, HaikuRepository>();
    }

    private static void AddLlm(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(LlmOptions.SectionName);
        services.AddOptions<LlmOptions>().Bind(section);

        var options = section.Get<LlmOptions>() ?? new LlmOptions();

        switch (options.Provider)
        {
            case LlmProvider.Mock:
                // Тестовый этап: без внешнего API.
                services.AddScoped<IHaikuComposer, MockHaikuComposer>();
                services.AddScoped<IHaikuCritic, MockHaikuCritic>();
                break;

            case LlmProvider.ApiFreeLlm:
                AddApiFreeLlm(services);
                break;

            default:
                AddOpenAiCompatible(services);
                break;
        }
    }

    private static void AddApiFreeLlm(IServiceCollection services)
    {
        services.AddOptions<LlmOptions>()
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey),
                "Для провайдера ApiFreeLlm нужен Llm:ApiKey (LLM__APIKEY).");

        services.AddHttpClient<ApiFreeLlmClient>((sp, http) =>
        {
            var llm = sp.GetRequiredService<IOptions<LlmOptions>>().Value;
            // У провайдера фиксированный эндпоинт /api/v1/chat.
            http.BaseAddress = new Uri("https://apifreellm.com/api/v1/");
            http.Timeout = TimeSpan.FromSeconds(llm.RequestTimeoutSeconds);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", llm.ApiKey);
        });

        services.AddScoped<IHaikuComposer, ApiFreeLlmComposer>();
        services.AddScoped<IHaikuCritic, ApiFreeLlmCritic>();
    }

    private static void AddOpenAiCompatible(IServiceCollection services)
    {
        services.AddOptions<LlmOptions>()
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey),
                "Для провайдера OpenAiCompatible нужен Llm:ApiKey (LLM__APIKEY).");

        services.AddHttpClient<ChatCompletionClient>((sp, http) =>
        {
            var llm = sp.GetRequiredService<IOptions<LlmOptions>>().Value;
            http.BaseAddress = new Uri(llm.BaseUrl.TrimEnd('/') + "/");
            http.Timeout = TimeSpan.FromSeconds(llm.RequestTimeoutSeconds);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", llm.ApiKey);
            // Рекомендуемые OpenRouter-заголовки (безвредны для других провайдеров).
            http.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/haiku-bot");
            http.DefaultRequestHeaders.Add("X-Title", "Haiku Telegram Bot");
        });

        services.AddScoped<IHaikuComposer, OpenAiHaikuComposer>();
        services.AddScoped<IHaikuCritic, OpenAiHaikuCritic>();
    }
}
