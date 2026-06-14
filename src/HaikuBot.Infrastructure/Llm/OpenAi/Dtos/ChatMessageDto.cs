using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.OpenAi.Dtos;

/// <summary>Сообщение в формате OpenAI chat/completions.</summary>
public sealed record ChatMessageDto(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);
