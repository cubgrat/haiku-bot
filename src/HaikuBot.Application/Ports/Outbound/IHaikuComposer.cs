namespace HaikuBot.Application.Ports.Outbound;

/// <summary>
/// Исходящий порт: «сочинитель» хайку (первая нейросеть).
/// Адаптеры — реальный LLM-клиент или мок.
/// </summary>
public interface IHaikuComposer
{
    /// <summary>Сочинить хайку по теме. <paramref name="criticHint"/> — замечание критика
    /// от предыдущей попытки (null при первой попытке).</summary>
    Task<string> ComposeAsync(string topic, string? criticHint, CancellationToken ct);
}
