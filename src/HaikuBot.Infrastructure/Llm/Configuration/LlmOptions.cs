namespace HaikuBot.Infrastructure.Llm.Configuration;

/// <summary>Настройки доступа к нейросетям.</summary>
public sealed class LlmOptions
{
    public const string SectionName = "Llm";

    /// <summary>Провайдер: Mock (по умолчанию, тестовый этап) или OpenAiCompatible.</summary>
    public LlmProvider Provider { get; set; } = LlmProvider.Mock;

    /// <summary>Базовый URL API (без /chat/completions).</summary>
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>API-ключ провайдера (нужен только для OpenAiCompatible).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Модель-сочинитель хайку.</summary>
    public string GeneratorModel { get; set; } = "meta-llama/llama-3.3-70b-instruct:free";

    /// <summary>Модель-критик, оценивающая качество хайку.</summary>
    public string JudgeModel { get; set; } = "google/gemini-2.0-flash-exp:free";

    /// <summary>Таймаут запроса к нейросети, секунды.</summary>
    public int RequestTimeoutSeconds { get; set; } = 60;
}
