namespace HaikuBot.Domain.Models;

/// <summary>Запрос пользователя — исходный текст, по которому сочиняем хайку.</summary>
public sealed class HaikuRequest
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string InputText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
