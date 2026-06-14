namespace HaikuBot.Application.Dtos;

/// <summary>Результат прогона пайплайна — то, что показываем пользователю.</summary>
public sealed record HaikuResult(
    long GenerationId,
    string Text,
    int? Score,
    bool Approved,
    int Attempts);
