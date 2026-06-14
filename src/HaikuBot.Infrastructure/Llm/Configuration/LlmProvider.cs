namespace HaikuBot.Infrastructure.Llm.Configuration;

/// <summary>Какой адаптер нейросетей использовать.</summary>
public enum LlmProvider
{
    /// <summary>Мок без обращения к внешнему API — для тестового этапа.</summary>
    Mock = 0,

    /// <summary>Реальный OpenAI-совместимый эндпоинт (OpenRouter, Groq, OpenAI, Ollama…).</summary>
    OpenAiCompatible = 1,

    /// <summary>Бесплатный провайдер apifreellm.com (свой простой формат /api/v1/chat).</summary>
    ApiFreeLlm = 2
}
