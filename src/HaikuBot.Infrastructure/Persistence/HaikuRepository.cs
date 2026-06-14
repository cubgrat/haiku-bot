using HaikuBot.Application.Ports.Outbound;
using HaikuBot.Domain.Models;
using HaikuBot.Infrastructure.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HaikuBot.Infrastructure.Persistence;

/// <summary>Адаптер хранилища поверх EF Core / PostgreSQL.</summary>
public sealed class HaikuRepository(AppDbContext db) : IHaikuRepository
{
    public async Task<HaikuRequest> AddRequestAsync(HaikuRequest request, CancellationToken ct)
    {
        var entity = request.ToEntity();
        db.Requests.Add(entity);
        await db.SaveChangesAsync(ct);
        request.Id = entity.Id; // переносим присвоенный БД идентификатор
        return request;
    }

    public async Task AddGenerationAsync(HaikuGeneration generation, CancellationToken ct)
    {
        var entity = generation.ToEntity();
        db.Generations.Add(entity);
        await db.SaveChangesAsync(ct);
        generation.Id = entity.Id;
    }

    public async Task AddFeedbackAsync(Feedback feedback, CancellationToken ct)
    {
        var entity = feedback.ToEntity();
        db.Feedbacks.Add(entity);
        await db.SaveChangesAsync(ct);
        feedback.Id = entity.Id;
    }

    public async Task<HaikuRequest?> GetRequestAsync(long requestId, CancellationToken ct)
    {
        var entity = await db.Requests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == requestId, ct);
        return entity?.ToDomain();
    }

    public async Task<HaikuGeneration?> GetGenerationAsync(long generationId, CancellationToken ct)
    {
        var entity = await db.Generations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == generationId, ct);
        return entity?.ToDomain();
    }
}
