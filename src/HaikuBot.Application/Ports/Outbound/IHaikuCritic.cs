using HaikuBot.Domain.Models;

namespace HaikuBot.Application.Ports.Outbound;

/// <summary>
/// Исходящий порт: «критик», оценивающий качество хайку (вторая нейросеть).
/// Адаптеры — реальный LLM-клиент или мок.
/// </summary>
public interface IHaikuCritic
{
    Task<JudgeVerdict> EvaluateAsync(string topic, string haiku, CancellationToken ct);
}
