namespace HaikuBot.Application.Dtos;

/// <summary>Команда «сочинить хайку» по новому сообщению пользователя.</summary>
public sealed record ComposeHaikuCommand(
    long ChatId,
    long UserId,
    string? Username,
    string Text);
