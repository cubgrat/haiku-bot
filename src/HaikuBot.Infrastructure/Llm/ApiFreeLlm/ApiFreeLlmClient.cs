using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HaikuBot.Infrastructure.Llm.ApiFreeLlm.Dtos;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm.ApiFreeLlm;

/// <summary>
/// Клиент к apifreellm.com (/api/v1/chat). Один эндпоинт принимает текст и
/// возвращает ответ модели. Обрабатывает rate limit (429): ждёт и повторяет.
/// </summary>
public sealed class ApiFreeLlmClient(HttpClient http, ILogger<ApiFreeLlmClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const int MaxRateLimitRetries = 3;
    private static readonly TimeSpan DefaultRateLimitWait = TimeSpan.FromSeconds(20);

    public async Task<string> ChatAsync(string message, CancellationToken ct)
    {
        for (var attempt = 0; ; attempt++)
        {
            using var response = await http.PostAsJsonAsync("chat", new ApiFreeLlmRequest(message), JsonOptions, ct);
            var payload = await ReadPayloadAsync(response, ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < MaxRateLimitRetries)
            {
                var wait = GetRetryDelay(response);
                logger.LogWarning("ApiFreeLLM rate limit (429), жду {Seconds:0}с и повторяю…", wait.TotalSeconds);
                await Task.Delay(wait, ct);
                continue;
            }

            if (!response.IsSuccessStatusCode || payload is { Success: false })
            {
                var error = payload?.Error ?? $"HTTP {(int)response.StatusCode}";
                logger.LogError("ApiFreeLLM ошибка ({Code}): {Error}", payload?.Code ?? (int)response.StatusCode, error);
                throw new HttpRequestException($"ApiFreeLLM error: {error}");
            }

            if (string.IsNullOrWhiteSpace(payload?.Response))
                throw new InvalidOperationException("ApiFreeLLM вернул пустой ответ.");

            return payload.Response.Trim();
        }
    }

    private static async Task<ApiFreeLlmResponse?> ReadPayloadAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<ApiFreeLlmResponse>(JsonOptions, ct);
        }
        catch (JsonException)
        {
            return null; // тело не JSON — обработаем по статус-коду
        }
    }

    private static TimeSpan GetRetryDelay(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta is { } delta && delta > TimeSpan.Zero)
            return delta;
        if (response.Headers.RetryAfter?.Date is { } date)
        {
            var span = date - DateTimeOffset.UtcNow;
            if (span > TimeSpan.Zero) return span;
        }
        return DefaultRateLimitWait;
    }
}
