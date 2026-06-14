using HaikuBot.Domain.Models;
using HaikuBot.Infrastructure.Persistence.Entities;

namespace HaikuBot.Infrastructure.Persistence.Mapping;

internal static class GenerationMapper
{
    public static GenerationEntity ToEntity(this HaikuGeneration m) => new()
    {
        Id = m.Id,
        RequestId = m.RequestId,
        Text = m.Text,
        AttemptNumber = m.AttemptNumber,
        Trigger = m.Trigger,
        CriticScore = m.CriticScore,
        Approved = m.Approved,
        CriticComment = m.CriticComment,
        CreatedAt = m.CreatedAt
    };

    public static HaikuGeneration ToDomain(this GenerationEntity e) => new()
    {
        Id = e.Id,
        RequestId = e.RequestId,
        Text = e.Text,
        AttemptNumber = e.AttemptNumber,
        Trigger = e.Trigger,
        CriticScore = e.CriticScore,
        Approved = e.Approved,
        CriticComment = e.CriticComment,
        CreatedAt = e.CreatedAt
    };
}
