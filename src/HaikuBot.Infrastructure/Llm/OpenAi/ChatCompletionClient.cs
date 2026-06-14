using System.Net.Http.Json;
using System.Text.Json;
using HaikuBot.Infrastructure.Llm.OpenAi.Dtos;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm.OpenAi;

/// <summary>
/// Тонкий клиент к OpenAI-совместимому эндпоинту /chat/completions.
/// Общий для адаптеров «сочинитель» и «критик».
/// </summary>
public sealed class ChatCompletionClient(HttpClient http, ILogger<ChatCompletionClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<string> CompleteAsync(
        string model,
        string systemPrompt,
        string userPrompt,
        double temperature,
        CancellationToken ct)
    {
        var request = new ChatCompletionRequest
        {
            Model = model,
            Temperature = temperature,
            Messages =
            [
                new ChatMessageDto("system", systemPrompt),
                new ChatMessageDto("user", userPrompt)
            ]
        };

        using var response = await http.PostAsJsonAsync("chat/completions", request, JsonOptions, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("LLM запрос к модели {Model} вернул {Status}: {Body}",
                model, (int)response.StatusCode, body);
            throw new HttpRequestException(
                $"LLM эндпоинт вернул {(int)response.StatusCode} для модели {model}.");
        }

        var parsed = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(JsonOptions, ct);
        var content = parsed?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException($"Пустой ответ от модели {model}.");

        return content.Trim();
    }
}
