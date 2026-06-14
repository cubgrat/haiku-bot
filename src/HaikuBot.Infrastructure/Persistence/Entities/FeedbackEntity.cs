using HaikuBot.Domain.Models;

namespace HaikuBot.Infrastructure.Persistence.Entities;

/// <summary>EF-сущность таблицы feedbacks (отделена от доменной модели).</summary>
public sealed class FeedbackEntity
{
    public long Id { get; set; }
    public long GenerationId { get; set; }
    public GenerationEntity? Generation { get; set; }

    public FeedbackType Type { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
