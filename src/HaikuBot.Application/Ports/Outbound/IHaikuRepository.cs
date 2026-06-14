using HaikuBot.Domain.Models;

namespace HaikuBot.Application.Ports.Outbound;

/// <summary>Исходящий порт хранилища. Адаптер — EF Core / PostgreSQL.</summary>
public interface IHaikuRepository
{
    /// <summary>Сохранить запрос; возвращает его с присвоенным Id.</summary>
    Task<HaikuRequest> AddRequestAsync(HaikuRequest request, CancellationToken ct);

    /// <summary>Сохранить генерацию; Id присваивается объекту.</summary>
    Task AddGenerationAsync(HaikuGeneration generation, CancellationToken ct);

    /// <summary>Сохранить реакцию пользователя.</summary>
    Task AddFeedbackAsync(Feedback feedback, CancellationToken ct);

    Task<HaikuRequest?> GetRequestAsync(long requestId, CancellationToken ct);

    Task<HaikuGeneration?> GetGenerationAsync(long generationId, CancellationToken ct);
}
