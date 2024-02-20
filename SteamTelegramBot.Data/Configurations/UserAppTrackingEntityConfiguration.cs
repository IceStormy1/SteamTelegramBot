using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Configurations;

internal sealed class UserAppTrackingEntityConfiguration : IEntityTypeConfiguration<UserAppTrackingEntity>
{
    public void Configure(EntityTypeBuilder<UserAppTrackingEntity> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.SteamAppId })
            .IsUnique();
    }
}