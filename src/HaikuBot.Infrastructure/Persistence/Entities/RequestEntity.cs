namespace HaikuBot.Infrastructure.Persistence.Entities;

/// <summary>EF-сущность таблицы requests (отделена от доменной модели).</summary>
public sealed class RequestEntity
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string InputText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public List<GenerationEntity> Generations { get; set; } = new();
}
