using HaikuBot.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaikuBot.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RequestEntity> Requests => Set<RequestEntity>();
    public DbSet<GenerationEntity> Generations => Set<GenerationEntity>();
    public DbSet<FeedbackEntity> Feedbacks => Set<FeedbackEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<RequestEntity>(e =>
        {
            e.ToTable("requests");
            e.HasKey(x => x.Id);
            e.Property(x => x.InputText).HasMaxLength(4096).IsRequired();
            e.Property(x => x.Username).HasMaxLength(256);
            e.HasIndex(x => x.ChatId);
        });

        b.Entity<GenerationEntity>(e =>
        {
            e.ToTable("generations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Text).HasMaxLength(4096).IsRequired();
            e.Property(x => x.CriticComment).HasMaxLength(2048);
            e.HasOne(x => x.Request)
                .WithMany(r => r.Generations)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.RequestId);
        });

        b.Entity<FeedbackEntity>(e =>
        {
            e.ToTable("feedbacks");
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Generation)
                .WithMany(g => g.Feedbacks)
                .HasForeignKey(x => x.GenerationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.GenerationId);
        });
    }
}
