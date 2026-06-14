namespace HaikuBot.Telegram.Configuration;

/// <summary>Настройки Telegram-адаптера.</summary>
public sealed class BotOptions
{
    public const string SectionName = "Bot";

    /// <summary>Токен бота от @BotFather.</summary>
    public string Token { get; set; } = string.Empty;
}
