using HaikuBot.Application.Dtos;
using HaikuBot.Application.Ports.Inbound;
using HaikuBot.Telegram.Presentation;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HaikuBot.Telegram.Handlers;

/// <summary>Обрабатывает один апдейт Telegram (создаётся в отдельном DI-scope).</summary>
public sealed class UpdateHandler(
    ITelegramBotClient bot,
    IHaikuService haikuService,
    ILogger<UpdateHandler> logger)
{
    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        try
        {
            switch (update)
            {
                case { Message: { Text: { } text } message }:
                    await HandleTextAsync(message, text, ct);
                    break;
                case { CallbackQuery: { } callback }:
                    await HandleCallbackAsync(callback, ct);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки апдейта {UpdateId}", update.Id);
        }
    }

    private async Task HandleTextAsync(Message message, string text, CancellationToken ct)
    {
        if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            await bot.SendMessage(message.Chat.Id,
                "Привет! Пришли мне любую тему, фразу или настроение — " +
                "и я сочиню по ним хайку. 🌸\n\n" +
                "Под каждым хайку будут кнопки 👍/👎: 👎 — и я попробую ещё раз.",
                cancellationToken: ct);
            return;
        }

        var thinking = await bot.SendMessage(message.Chat.Id, "✍️ Сочиняю хайку…", cancellationToken: ct);

        var command = new ComposeHaikuCommand(
            message.Chat.Id,
            message.From?.Id ?? 0,
            message.From?.Username,
            text.Trim());

        var result = await haikuService.ComposeAsync(command, ct);

        await bot.EditMessageText(
            message.Chat.Id,
            thinking.MessageId,
            MessagePresenter.BuildHaikuText(result),
            replyMarkup: MessagePresenter.BuildFeedbackKeyboard(result.GenerationId),
            cancellationToken: ct);
    }

    private async Task HandleCallbackAsync(CallbackQuery callback, CancellationToken ct)
    {
        var data = callback.Data ?? string.Empty;
        var message = callback.Message;
        if (message is null)
        {
            await bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
            return;
        }

        if (data.StartsWith(MessagePresenter.LikePrefix, StringComparison.Ordinal)
            && long.TryParse(data.AsSpan(MessagePresenter.LikePrefix.Length), out var likeId))
        {
            await HandleLikeAsync(callback, message, likeId, ct);
        }
        else if (data.StartsWith(MessagePresenter.DislikePrefix, StringComparison.Ordinal)
                 && long.TryParse(data.AsSpan(MessagePresenter.DislikePrefix.Length), out var dislikeId))
        {
            await HandleDislikeAsync(callback, message, dislikeId, ct);
        }
        else
        {
            await bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
        }
    }

    private async Task HandleLikeAsync(CallbackQuery callback, Message message, long generationId, CancellationToken ct)
    {
        var result = await haikuService.ApproveAsync(generationId, callback.From.Id, ct);
        if (result is null)
        {
            await bot.AnswerCallbackQuery(callback.Id, "Не нашёл это хайку 🤔", cancellationToken: ct);
            return;
        }

        // Убираем кнопки и помечаем хайку одобренным — «всё остаётся зелёным».
        await bot.EditMessageText(
            message.Chat.Id,
            message.MessageId,
            MessagePresenter.BuildApprovedText(result),
            replyMarkup: null,
            cancellationToken: ct);
        await bot.AnswerCallbackQuery(callback.Id, "Рад, что понравилось! 🌸", cancellationToken: ct);
    }

    private async Task HandleDislikeAsync(CallbackQuery callback, Message message, long generationId, CancellationToken ct)
    {
        await bot.AnswerCallbackQuery(callback.Id, "Хорошо, сочиняю заново… ✍️", cancellationToken: ct);

        var result = await haikuService.RegenerateAsync(generationId, callback.From.Id, ct);
        if (result is null)
        {
            await bot.SendMessage(message.Chat.Id, "Не нашёл исходный запрос для этого хайку 🤔", cancellationToken: ct);
            return;
        }

        await bot.EditMessageText(
            message.Chat.Id,
            message.MessageId,
            MessagePresenter.BuildHaikuText(result),
            replyMarkup: MessagePresenter.BuildFeedbackKeyboard(result.GenerationId),
            cancellationToken: ct);
    }
}
