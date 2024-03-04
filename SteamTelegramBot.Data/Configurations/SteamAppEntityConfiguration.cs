using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Configurations;

internal sealed class SteamAppEntityConfiguration : IEntityTypeConfiguration<SteamAppEntity>
{
    public void Configure(EntityTypeBuilder<SteamAppEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired();

        builder.HasIndex(x => x.SteamAppId).IsUnique();

        builder.HasMany(x => x.TrackedUsers)
            .WithOne(x => x.SteamApp)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PriceHistory)
            .WithOne(x => x.SteamApp)
            .OnDelete(DeleteBehavior.Cascade);
    }
}