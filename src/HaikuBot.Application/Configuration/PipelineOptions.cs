namespace HaikuBot.Application.Configuration;

/// <summary>Настройки пайплайна генерации/оценки.</summary>
public sealed class PipelineOptions
{
    public const string SectionName = "Pipeline";

    /// <summary>Сколько раз пробуем сгенерировать хайку, пока критик не одобрит.</summary>
    public int MaxGenerationAttempts { get; set; } = 3;

    /// <summary>Минимальная оценка критика (0-10), при которой хайку считается одобренным.</summary>
    public int ApprovalThreshold { get; set; } = 7;
}
