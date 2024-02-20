using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(UserConstants.FirstNameMaxLength);

        builder.Property(x => x.LastName)
            .HasMaxLength(UserConstants.LastNameMaxLength);
        
        builder.Property(x => x.Username)
            .HasMaxLength(UserConstants.UserNameMaxLength);

        builder.HasIndex(x => x.TelegramId).IsUnique();

        builder.HasMany(x => x.TrackedApps)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}