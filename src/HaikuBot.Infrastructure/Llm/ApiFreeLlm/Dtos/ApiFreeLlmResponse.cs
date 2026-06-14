using System.Text.Json.Serialization;

namespace HaikuBot.Infrastructure.Llm.ApiFreeLlm.Dtos;

/// <summary>
/// Ответ apifreellm.com. Успех: {"success":true,"response":"..."}.
/// Ошибка: {"success":false,"error":"...","code":401}.
/// </summary>
public sealed record ApiFreeLlmResponse
{
    [JsonPropertyName("success")] public bool Success { get; init; }
    [JsonPropertyName("response")] public string? Response { get; init; }
    [JsonPropertyName("error")] public string? Error { get; init; }
    [JsonPropertyName("code")] public int? Code { get; init; }
}
