using HaikuBot.Application.Dtos;
using Telegram.Bot.Types.ReplyMarkups;

namespace HaikuBot.Telegram.Presentation;

/// <summary>Формирование текста сообщений и клавиатур для Telegram.</summary>
public static class MessagePresenter
{
    public const string LikePrefix = "like:";
    public const string DislikePrefix = "dislike:";

    public static string BuildHaikuText(HaikuResult result)
    {
        var score = result.Score is int s ? $"  ·  оценка критика: {s}/10" : string.Empty;
        return $"""
                🌸 {result.Text}

                👍 — нравится  /  👎 — попробовать ещё{score}
                """;
    }

    public static string BuildApprovedText(HaikuResult result) =>
        $"""
         🌸 {result.Text}

         ✅ Хайку одобрено. Спасибо за отклик!
         """;

    public static InlineKeyboardMarkup BuildFeedbackKeyboard(long generationId) =>
        new(new[]
        {
            InlineKeyboardButton.WithCallbackData("👍 Нравится", $"{LikePrefix}{generationId}"),
            InlineKeyboardButton.WithCallbackData("👎 Ещё вариант", $"{DislikePrefix}{generationId}")
        });
}
