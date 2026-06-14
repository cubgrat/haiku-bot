using System.Text.Json;
using HaikuBot.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm;

/// <summary>Разбор JSON-вердикта критика, общий для всех LLM-провайдеров.</summary>
internal static class VerdictParser
{
    public static JudgeVerdict Parse(string raw, ILogger logger)
    {
        var json = ExtractJson(raw);
        if (json is null)
        {
            logger.LogWarning("Не удалось найти JSON в ответе критика: {Raw}", raw);
            return new JudgeVerdict(null, false, "Ответ критика не распознан.");
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            int? score = root.TryGetProperty("score", out var s) && s.TryGetInt32(out var v)
                ? Math.Clamp(v, 0, 10)
                : null;

            var approved = root.TryGetProperty("approved", out var a) && a.ValueKind == JsonValueKind.True;
            var comment = root.TryGetProperty("comment", out var c) ? c.GetString() : null;

            return new JudgeVerdict(score, approved, comment);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Ошибка парсинга JSON критика: {Raw}", raw);
            return new JudgeVerdict(null, false, "Ответ критика не распознан.");
        }
    }

    /// <summary>Достаёт JSON-объект, даже если модель обернула его в текст или ```.</summary>
    private static string? ExtractJson(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        return start >= 0 && end > start ? raw[start..(end + 1)] : null;
    }
}
