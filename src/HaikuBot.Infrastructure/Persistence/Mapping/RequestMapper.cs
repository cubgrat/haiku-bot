using HaikuBot.Domain.Models;
using HaikuBot.Infrastructure.Persistence.Entities;

namespace HaikuBot.Infrastructure.Persistence.Mapping;

internal static class RequestMapper
{
    public static RequestEntity ToEntity(this HaikuRequest m) => new()
    {
        Id = m.Id,
        ChatId = m.ChatId,
        UserId = m.UserId,
        Username = m.Username,
        InputText = m.InputText,
        CreatedAt = m.CreatedAt
    };

    public static HaikuRequest ToDomain(this RequestEntity e) => new()
    {
        Id = e.Id,
        ChatId = e.ChatId,
        UserId = e.UserId,
        Username = e.Username,
        InputText = e.InputText,
        CreatedAt = e.CreatedAt
    };
}
