using HaikuBot.Domain.Models;
using HaikuBot.Infrastructure.Persistence.Entities;

namespace HaikuBot.Infrastructure.Persistence.Mapping;

internal static class FeedbackMapper
{
    public static FeedbackEntity ToEntity(this Feedback m) => new()
    {
        Id = m.Id,
        GenerationId = m.GenerationId,
        Type = m.Type,
        UserId = m.UserId,
        CreatedAt = m.CreatedAt
    };

    public static Feedback ToDomain(this FeedbackEntity e) => new()
    {
        Id = e.Id,
        GenerationId = e.GenerationId,
        Type = e.Type,
        UserId = e.UserId,
        CreatedAt = e.CreatedAt
    };
}
