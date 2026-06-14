using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HaikuBot.Infrastructure.Persistence;

/// <summary>Подготовка БД при старте: создание схемы с повторами (Postgres может стартовать дольше).</summary>
public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IHost host, CancellationToken ct = default)
    {
        var logger = host.Services.GetRequiredService<ILogger<AppDbContext>>();
        const int maxAttempts = 15;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.EnsureCreatedAsync(ct);
                logger.LogInformation("База данных готова.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning("БД ещё не готова (попытка {Attempt}/{Max}): {Message}",
                    attempt, maxAttempts, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }
    }
}
