using System.Collections.Concurrent;

namespace HaikuBot.Telegram.Hosting;

/// <summary>
/// Отмечает сообщения, для которых уже идёт генерация, чтобы повторные нажатия
/// кнопок не плодили лишние запросы к нейросети (узкое место — бесплатный API).
/// </summary>
public sealed class InFlightTracker
{
    private readonly ConcurrentDictionary<string, byte> _busy = new();

    /// <summary>Пытается занять сообщение. false — уже обрабатывается.</summary>
    public bool TryBegin(long chatId, int messageId) => _busy.TryAdd(Key(chatId, messageId), 0);

    public void End(long chatId, int messageId) => _busy.TryRemove(Key(chatId, messageId), out _);

    private static string Key(long chatId, int messageId) => $"{chatId}:{messageId}";
}
