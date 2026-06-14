using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.OpenAi.Dtos;

/// <summary>Тело запроса к OpenAI-совместимому /chat/completions.</summary>
public sealed record ChatCompletionRequest
{
    [JsonPropertyName("model")] public string Model { get; init; } = string.Empty;
    [JsonPropertyName("messages")] public IReadOnlyList<ChatMessageDto> Messages { get; init; } = [];
    [JsonPropertyName("temperature")] public double Temperature { get; init; }
}
