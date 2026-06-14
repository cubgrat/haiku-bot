using HaikuBot.Application.Ports.Outbound;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm.Mock;

/// <summary>
/// Мок «сочинителя»: собирает псевдо-хайку из шаблонов по теме пользователя,
/// без обращения к внешнему API. Каждый вызов даёт другой вариант, чтобы
/// можно было увидеть работу повторных попыток.
/// </summary>
public sealed class MockHaikuComposer(ILogger<MockHaikuComposer> logger) : IHaikuComposer
{
    private static readonly string[] Openings =
        ["Тихо опустился", "В утренней тиши", "Сквозь туман плывёт", "На ладони дня", "Где-то у реки"];

    private static readonly string[] Middles =
        ["шёпот ветра над водой —", "след росы на лепестках,", "крылья птицы в вышине,", "отблеск лунного огня,", "эхо давних голосов,"];

    private static readonly string[] Closings =
        ["мир замер на миг.", "и тает печаль.", "дышит тишина.", "сон наяву.", "память хранит свет."];

    public Task<string> ComposeAsync(string topic, string? criticHint, CancellationToken ct)
    {
        var rnd = Random.Shared;
        var topicWord = FirstWord(topic);

        var haiku =
            $"{Openings[rnd.Next(Openings.Length)]} {topicWord},\n" +
            $"{Middles[rnd.Next(Middles.Length)]}\n" +
            $"{Closings[rnd.Next(Closings.Length)]}";

        logger.LogInformation("[MOCK] Сочинено хайку (hint={HasHint})", criticHint is not null);
        return Task.FromResult(haiku);
    }

    private static string FirstWord(string topic)
    {
        var word = topic.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "тишина";
        return word.ToLowerInvariant();
    }
}
