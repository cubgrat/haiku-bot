using HaikuBot.Application.Dtos;

namespace HaikuBot.Application.Ports.Inbound;

/// <summary>
/// Входящий порт (use-cases приложения). Через него driving-адаптеры
/// (например, Telegram) управляют сценариями работы с хайку.
/// </summary>
public interface IHaikuService
{
    /// <summary>Сочинить хайку по новому сообщению пользователя.</summary>
    Task<HaikuResult> ComposeAsync(ComposeHaikuCommand command, CancellationToken ct);

    /// <summary>Отметить дизлайк и сгенерировать новый вариант. null — если генерация не найдена.</summary>
    Task<HaikuResult?> RegenerateAsync(long generationId, long userId, CancellationToken ct);

    /// <summary>Отметить лайк и зафиксировать одобренное хайку. null — если генерация не найдена.</summary>
    Task<HaikuResult?> ApproveAsync(long generationId, long userId, CancellationToken ct);
}
