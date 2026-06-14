using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.OpenAi.Dtos;

/// <summary>Ответ OpenAI-совместимого /chat/completions.</summary>
public sealed record ChatCompletionResponse
{
    [JsonPropertyName("choices")] public IReadOnlyList<ChatChoiceDto>? Choices { get; init; }
}
