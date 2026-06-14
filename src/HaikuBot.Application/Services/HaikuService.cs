using System.Diagnostics;
using HaikuBot.Application.Configuration;
using HaikuBot.Application.Dtos;
using HaikuBot.Application.Ports.Inbound;
using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HaikuBot.Application.Services;

/// <summary>
/// Реализация входящего порта: оркестрация «сочинить → оценить → повторить»,
/// сохранение всех попыток и обработка обратной связи.
/// </summary>
public sealed class HaikuService(
    IHaikuComposer composer,
    IHaikuCritic critic,
    IHaikuRepository repository,
    IOptions<PipelineOptions> options,
    ILogger<HaikuService> logger) : IHaikuService
{
    private readonly PipelineOptions _options = options.Value;

    public async Task<HaikuResult> ComposeAsync(ComposeHaikuCommand command, CancellationToken ct)
    {
        var request = await repository.AddRequestAsync(new HaikuRequest
        {
            ChatId = command.ChatId,
            UserId = command.UserId,
            Username = command.Username,
            InputText = command.Text
        }, ct);

        return await RunPipelineAsync(request, GenerationTrigger.Initial, ct);
    }

    public async Task<HaikuResult?> RegenerateAsync(long generationId, long userId, CancellationToken ct)
    {
        var generation = await repository.GetGenerationAsync(generationId, ct);
        if (generation is null)
            return null;

        await repository.AddFeedbackAsync(new Feedback
        {
            GenerationId = generationId,
            Type = FeedbackType.Dislike,
            UserId = userId
        }, ct);

        var request = await repository.GetRequestAsync(generation.RequestId, ct);
        if (request is null)
            return null;

        return await RunPipelineAsync(request, GenerationTrigger.Regeneration, ct);
    }

    public async Task<HaikuResult?> ApproveAsync(long generationId, long userId, CancellationToken ct)
    {
        var generation = await repository.GetGenerationAsync(generationId, ct);
        if (generation is null)
            return null;

        await repository.AddFeedbackAsync(new Feedback
        {
            GenerationId = generationId,
            Type = FeedbackType.Like,
            UserId = userId
        }, ct);

        return ToResult(generation, approved: true, attempts: generation.AttemptNumber, elapsedMs: 0);
    }

    public async Task<IReadOnlyList<HaikuHistoryItem>> GetLikedHistoryAsync(long userId, int limit, CancellationToken ct)
    {
        var generations = await repository.GetLikedGenerationsAsync(userId, limit, ct);
        return generations
            .Select(g => new HaikuHistoryItem(g.Text, g.CreatedAt, g.CriticScore))
            .ToList();
    }

    private async Task<HaikuResult> RunPipelineAsync(HaikuRequest request, GenerationTrigger trigger, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var maxAttempts = Math.Max(1, _options.MaxGenerationAttempts);
        HaikuGeneration? best = null;
        string? criticHint = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var text = await composer.ComposeAsync(request.InputText, criticHint, ct);
            var verdict = await critic.EvaluateAsync(request.InputText, text, ct);

            var approved = verdict.Approved
                           || (verdict.Score is int score && score >= _options.ApprovalThreshold);

            var generation = new HaikuGeneration
            {
                RequestId = request.Id,
                Text = text,
                AttemptNumber = attempt,
                Trigger = trigger,
                CriticScore = verdict.Score,
                Approved = approved,
                CriticComment = verdict.Comment
            };
            await repository.AddGenerationAsync(generation, ct);

            logger.LogInformation(
                "Запрос {RequestId}, попытка {Attempt}/{Max}: score={Score}, approved={Approved}",
                request.Id, attempt, maxAttempts, verdict.Score, approved);

            if (best is null || (generation.CriticScore ?? -1) > (best.CriticScore ?? -1))
                best = generation;

            if (approved)
                return ToResult(generation, approved: true, attempt, stopwatch.ElapsedMilliseconds);

            criticHint = verdict.Comment;
        }

        // Ни одна попытка не одобрена — отдаём лучшую по оценке.
        return ToResult(best!, approved: false, maxAttempts, stopwatch.ElapsedMilliseconds);
    }

    private static HaikuResult ToResult(HaikuGeneration generation, bool approved, int attempts, long elapsedMs) =>
        new(generation.Id, generation.Text, generation.CriticScore, approved, attempts, elapsedMs);
}
