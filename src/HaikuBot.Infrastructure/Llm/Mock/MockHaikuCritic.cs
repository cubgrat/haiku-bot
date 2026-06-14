using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm.Mock;

/// <summary>
/// Мок «критика»: выставляет детерминированную по тексту оценку 0-10,
/// без обращения к внешнему API. Разные хайку получают разные оценки,
/// поэтому повторные попытки в пайплайне реально срабатывают.
/// </summary>
public sealed class MockHaikuCritic(ILogger<MockHaikuCritic> logger) : IHaikuCritic
{
    public Task<JudgeVerdict> EvaluateAsync(string topic, string haiku, CancellationToken ct)
    {
        // Стабильный хэш текста → оценка 0..10.
        var score = (int)(StableHash(haiku) % 11u);
        var comment = score >= 7
            ? "[MOCK] Образ цельный, ритм выдержан."
            : "[MOCK] Образ размытый, не хватает сезонной детали.";

        logger.LogInformation("[MOCK] Оценка критика: {Score}/10", score);
        // Решение об одобрении принимает приложение по порогу — критик отдаёт оценку.
        return Task.FromResult(new JudgeVerdict(score, Approved: false, comment));
    }

    private static uint StableHash(string s)
    {
        // FNV-1a — стабилен между запусками (в отличие от string.GetHashCode).
        uint hash = 2166136261;
        foreach (var c in s)
        {
            hash ^= c;
            hash *= 16777619;
        }
        return hash;
    }
}
