using HaikuBot.Domain.Models;

namespace HaikuBot.Infrastructure.Persistence.Entities;

/// <summary>EF-сущность таблицы generations (отделена от доменной модели).</summary>
public sealed class GenerationEntity
{
    public long Id { get; set; }
    public long RequestId { get; set; }
    public RequestEntity? Request { get; set; }

    public string Text { get; set; } = string.Empty;
    public int AttemptNumber { get; set; }
    public GenerationTrigger Trigger { get; set; }

    public int? CriticScore { get; set; }
    public bool Approved { get; set; }
    public string? CriticComment { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<FeedbackEntity> Feedbacks { get; set; } = new();
}
