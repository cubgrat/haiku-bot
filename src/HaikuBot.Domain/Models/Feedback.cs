namespace HaikuBot.Domain.Models;

/// <summary>Реакция пользователя (лайк/дизлайк) на конкретную генерацию.</summary>
public sealed class Feedback
{
    public long Id { get; set; }
    public long GenerationId { get; set; }
    public FeedbackType Type { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
