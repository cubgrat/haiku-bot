namespace HaikuBot.Domain.Models;

/// <summary>Причина появления генерации.</summary>
public enum GenerationTrigger
{
    /// <summary>Первичная генерация по сообщению пользователя.</summary>
    Initial = 0,

    /// <summary>Повторная генерация после дизлайка.</summary>
    Regeneration = 1
}
