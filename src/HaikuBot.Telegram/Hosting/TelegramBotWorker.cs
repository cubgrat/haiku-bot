using HaikuBot.Telegram.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HaikuBot.Telegram.Hosting;

/// <summary>Driving-адаптер: слушает апдейты Telegram long-polling'ом.</summary>
public sealed class TelegramBotWorker(
    ITelegramBotClient bot,
    IServiceScopeFactory scopeFactory,
    ILogger<TelegramBotWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await bot.GetMe(stoppingToken);
        logger.LogInformation("Бот @{Username} запущен и слушает сообщения.", me.Username);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = true
        };

        await bot.ReceiveAsync(HandleUpdateAsync, HandleErrorAsync, receiverOptions, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken ct)
    {
        // Свой scope на каждый апдейт — чтобы DbContext был «свежим» и потокобезопасным.
        await using var scope = scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
        await handler.HandleAsync(update, ct);
    }

    private Task HandleErrorAsync(ITelegramBotClient _, Exception ex, CancellationToken ct)
    {
        logger.LogError(ex, "Ошибка long-polling Telegram.");
        return Task.CompletedTask;
    }
}
