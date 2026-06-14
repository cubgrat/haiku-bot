using HaikuBot.Application.Ports.Outbound;

namespace HaikuBot.Infrastructure.Llm.ApiFreeLlm;

/// <summary>Адаптер «сочинителя» поверх apifreellm.com.</summary>
public sealed class ApiFreeLlmComposer(ApiFreeLlmClient client) : IHaikuComposer
{
    public Task<string> ComposeAsync(string topic, string? criticHint, CancellationToken ct)
    {
        // У провайдера одно поле message — складываем инструкции и тему в один текст.
        var message =
            $"""
             Ты — мастер японской поэзии хайку, пишущий на русском языке.
             Сочини одно хайку по теме: «{topic}».
             Требования: три строки, образность природы и момента, по канону близко к 5-7-5 слогов.
             Верни ТОЛЬКО три строки хайку, каждую с новой строки, без нумерации, кавычек и пояснений.
             """;

        if (!string.IsNullOrWhiteSpace(criticHint))
            message += $"\n\nПредыдущая попытка отклонена критиком. Замечание: {criticHint}\nСочини новое, более удачное хайку с учётом замечания.";

        return client.ChatAsync(message, ct);
    }
}
