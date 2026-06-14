using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Domain.Models;
using HaikuBot.Infrastructure.Llm.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HaikuBot.Infrastructure.Llm.OpenAi;

/// <summary>Адаптер «критика» поверх реального LLM. Возвращает JSON-вердикт.</summary>
public sealed class OpenAiHaikuCritic(
    ChatCompletionClient client,
    IOptions<LlmOptions> options,
    ILogger<OpenAiHaikuCritic> logger) : IHaikuCritic
{
    private readonly LlmOptions _options = options.Value;

    private const string SystemPrompt =
        """
        Ты — строгий литературный критик, оценивающий хайку на русском языке.
        Критерии: соответствие теме, образность, сезонный образ или яркий момент,
        близость к ритму 5-7-5, отсутствие банальности.
        Верни ответ СТРОГО в формате JSON без markdown и лишнего текста:
        {"score": <целое 0-10>, "approved": <true|false>, "comment": "<краткое замечание на русском>"}
        approved=true ставь только если хайку действительно хорошее.
        """;

    public async Task<JudgeVerdict> EvaluateAsync(string topic, string haiku, CancellationToken ct)
    {
        var prompt =
            $"""
             Тема пользователя: {topic}

             Хайку на оценку:
             {haiku}
             """;

        var raw = await client.CompleteAsync(_options.JudgeModel, SystemPrompt, prompt, 0.2, ct);
        return VerdictParser.Parse(raw, logger);
    }
}
