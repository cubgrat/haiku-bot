namespace HaikuBot.Application.Dtos;

/// <summary>Элемент истории удачных (понравившихся пользователю) хайку.</summary>
public sealed record HaikuHistoryItem(
    string Text,
    DateTimeOffset CreatedAt,
    int? Score);
