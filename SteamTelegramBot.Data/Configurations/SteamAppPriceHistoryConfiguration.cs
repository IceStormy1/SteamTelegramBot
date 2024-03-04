using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Configurations;

internal sealed class SteamAppPriceHistoryConfiguration : IEntityTypeConfiguration<SteamAppPriceHistoryEntity>
{
    public void Configure(EntityTypeBuilder<SteamAppPriceHistoryEntity> builder)
    {
        builder.HasIndex(x => new { x.SteamAppId, x.Version })
            .IsUnique();

        builder.HasMany(x => x.TelegramNotifications)
            .WithOne(x => x.SteamAppPrice)
            .OnDelete(DeleteBehavior.Cascade);
    }
}