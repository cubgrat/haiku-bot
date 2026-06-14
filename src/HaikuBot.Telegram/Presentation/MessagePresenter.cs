using System.Globalization;
using HaikuBot.Application.Dtos;
using Telegram.Bot.Types.ReplyMarkups;

namespace HaikuBot.Telegram.Presentation;

/// <summary>Формирование текста сообщений и клавиатур для Telegram.</summary>
public static class MessagePresenter
{
    public const string LikePrefix = "like:";
    public const string DislikePrefix = "dislike:";

    /// <summary>Подпись кнопки нижней клавиатуры для запроса истории.</summary>
    public const string HistoryButtonLabel = "📜 История удачных";

    /// <summary>Текст-индикатор, пока сочиняется новый вариант (без кнопок).</summary>
    public const string RegeneratingText = "⏳ Сочиняю новый вариант…";

    public static string BuildHaikuText(HaikuResult result)
    {
        var score = result.Score is int s ? $"  ·  оценка критика: {s}/10" : string.Empty;
        var time = result.ElapsedMs > 0
            ? $"\n⏱ сгенерировано за {result.ElapsedMs / 1000.0:0.0} с"
            : string.Empty;
        return $"""
                🌸 {result.Text}

                👍 — нравится  /  👎 — ещё вариант{score}{time}
                """;
    }

    public static string BuildApprovedText(HaikuResult result) =>
        $"""
         🌸 {result.Text}

         ✅ Хайку одобрено и добавлено в «{HistoryButtonLabel}». Спасибо за отклик!
         """;

    public static string BuildHistoryText(IReadOnlyList<HaikuHistoryItem> items)
    {
        if (items.Count == 0)
            return "Пока пусто. Поставь 👍 под понравившимся хайку — и оно попадёт в историю удачных.";

        var lines = items.Select((item, i) =>
        {
            var date = item.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            return $"{i + 1}. 🌸 {item.Text}\n   ({date})";
        });

        return $"📜 Твои удачные хайку ({items.Count}):\n\n" + string.Join("\n\n", lines);
    }

    public static InlineKeyboardMarkup BuildFeedbackKeyboard(long generationId) =>
        new(new[]
        {
            InlineKeyboardButton.WithCallbackData("👍 Нравится", $"{LikePrefix}{generationId}"),
            InlineKeyboardButton.WithCallbackData("👎 Ещё вариант", $"{DislikePrefix}{generationId}")
        });

    /// <summary>Постоянная нижняя клавиатура с кнопкой истории.</summary>
    public static ReplyKeyboardMarkup BuildMainKeyboard() =>
        new(new[] { new KeyboardButton(HistoryButtonLabel) })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };
}
