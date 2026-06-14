namespace HaikuBot.Domain.Models;

/// <summary>Одна сгенерированная версия хайку вместе с вердиктом критика.</summary>
public sealed class HaikuGeneration
{
    public long Id { get; set; }
    public long RequestId { get; set; }

    public string Text { get; set; } = string.Empty;

    /// <summary>Номер попытки в рамках одного прогона пайплайна (1..N).</summary>
    public int AttemptNumber { get; set; }

    public GenerationTrigger Trigger { get; set; }

    /// <summary>Оценка критика 0-10 (null, если распарсить не удалось).</summary>
    public int? CriticScore { get; set; }

    public bool Approved { get; set; }

    public string? CriticComment { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
