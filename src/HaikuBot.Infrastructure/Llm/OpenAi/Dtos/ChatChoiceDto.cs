using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.OpenAi.Dtos;

/// <summary>Один вариант ответа модели.</summary>
public sealed record ChatChoiceDto
{
    [JsonPropertyName("message")] public ChatMessageDto? Message { get; init; }
}
