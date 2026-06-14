namespace HaikuBot.Domain.Models;

/// <summary>Вердикт критика по одному хайку.</summary>
/// <param name="Score">Оценка 0-10 или null, если её не удалось определить.</param>
/// <param name="Approved">Рекомендация критика одобрить хайку.</param>
/// <param name="Comment">Краткое текстовое замечание.</param>
public sealed record JudgeVerdict(int? Score, bool Approved, string? Comment);
