using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Infrastructure.Llm.Configuration;
using Microsoft.Extensions.Options;

namespace HaikuBot.Infrastructure.Llm.OpenAi;

/// <summary>Адаптер «сочинителя» поверх реального LLM.</summary>
public sealed class OpenAiHaikuComposer(ChatCompletionClient client, IOptions<LlmOptions> options) : IHaikuComposer
{
    private readonly LlmOptions _options = options.Value;

    private const string SystemPrompt =
        """
        Ты — поэт, мастер японской поэзии хайку, пишущий на русском языке.
        По заданной теме сочини одно хайку: три строки, образность природы и момента,
        по канону близко к схеме 5-7-5 слогов.
        Правила ответа:
        - Верни ТОЛЬКО три строки хайку, каждую с новой строки.
        - Без нумерации, кавычек, пояснений и заголовков.
        """;

    public Task<string> ComposeAsync(string topic, string? criticHint, CancellationToken ct)
    {
        var prompt = string.IsNullOrWhiteSpace(criticHint)
            ? $"Тема: {topic}"
            : $"""
               Тема: {topic}

               Предыдущая попытка отклонена критиком. Замечание: {criticHint}
               Сочини новое, более удачное хайку с учётом замечания.
               """;

        var temperature = string.IsNullOrWhiteSpace(criticHint) ? 0.9 : 1.0;
        return client.CompleteAsync(_options.GeneratorModel, SystemPrompt, prompt, temperature, ct);
    }
}
