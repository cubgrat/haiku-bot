using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Llm.ApiFreeLlm;

/// <summary>Адаптер «критика» поверх apifreellm.com. Просит вернуть JSON-вердикт.</summary>
public sealed class ApiFreeLlmCritic(ApiFreeLlmClient client, ILogger<ApiFreeLlmCritic> logger) : IHaikuCritic
{
    public async Task<JudgeVerdict> EvaluateAsync(string topic, string haiku, CancellationToken ct)
    {
        var message =
            $$"""
              Ты — строгий литературный критик, оценивающий хайку на русском языке.
              Критерии: соответствие теме, образность, сезонный образ или яркий момент,
              близость к ритму 5-7-5, отсутствие банальности.
              Верни ответ СТРОГО в формате JSON без markdown и лишнего текста:
              {"score": <целое 0-10>, "approved": <true|false>, "comment": "<краткое замечание на русском>"}
              approved=true ставь только если хайку действительно хорошее.

              Тема пользователя: {{topic}}

              Хайку на оценку:
              {{haiku}}
              """;

        var raw = await client.ChatAsync(message, ct);
        return VerdictParser.Parse(raw, logger);
    }
}
