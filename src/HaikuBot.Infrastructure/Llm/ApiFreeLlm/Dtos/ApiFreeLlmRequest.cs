using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.ApiFreeLlm.Dtos;

/// <summary>Тело запроса apifreellm.com: единственное поле message.</summary>
public sealed record ApiFreeLlmRequest(
    [property: JsonPropertyName("message")] string Message);
