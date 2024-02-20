using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Data.Configurations;
using SteamTelegramBot.Data.Entities;
using System.Reflection;

namespace SteamTelegramBot.Data;

public sealed class SteamTelegramBotDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SteamAppEntity> SteamApps { get; set; }
    public DbSet<UserAppTrackingEntity> UserTrackedApps { get; set; }

    public SteamTelegramBotDbContext(DbContextOptions options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        SetDates();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserConfiguration))!);
    }

    private void SetDates()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                (e.Entity is IHasCreatedAt or IHasUpdatedAt) && (e.State is EntityState.Added or EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added && entityEntry.Entity is IHasCreatedAt createdEntity)
                createdEntity.CreatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Modified && entityEntry.Entity is IHasUpdatedAt modifiedEntity)
                modifiedEntity.UpdatedAt = DateTime.UtcNow;
        }
    }
}