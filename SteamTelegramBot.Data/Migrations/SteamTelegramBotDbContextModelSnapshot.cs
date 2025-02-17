﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SteamTelegramBot.Data;

#nullable disable

namespace SteamTelegramBot.Data.Migrations
{
    [DbContext(typeof(SteamTelegramBotDbContext))]
    partial class SteamTelegramBotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.SteamAppEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("HeaderImage")
                        .HasColumnType("text");

                    b.Property<int>("SteamAppId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SteamAppId")
                        .IsUnique();

                    b.ToTable("SteamApps");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.SteamAppPriceHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal?>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("PriceType")
                        .HasColumnType("integer");

                    b.Property<long>("SteamAppId")
                        .HasColumnType("bigint");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SteamAppId", "Version")
                        .IsUnique();

                    b.ToTable("SteamAppPriceHistory");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.TelegramNotificationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("SteamAppPriceId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserAppTrackingId")
                        .HasColumnType("bigint");

                    b.Property<bool>("WasSent")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("SteamAppPriceId");

                    b.HasIndex("UserAppTrackingId");

                    b.ToTable("TelegramNotifications");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.UserAppTrackingEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("SteamAppId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SteamAppId");

                    b.HasIndex("UserId", "SteamAppId")
                        .IsUnique();

                    b.ToTable("UserTrackedApps");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("character varying(70)");

                    b.Property<string>("LastName")
                        .HasMaxLength(70)
                        .HasColumnType("character varying(70)");

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasMaxLength(70)
                        .HasColumnType("character varying(70)");

                    b.HasKey("Id");

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.SteamAppPriceHistoryEntity", b =>
                {
                    b.HasOne("SteamTelegramBot.Data.Entities.SteamAppEntity", "SteamApp")
                        .WithMany("PriceHistory")
                        .HasForeignKey("SteamAppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SteamApp");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.TelegramNotificationEntity", b =>
                {
                    b.HasOne("SteamTelegramBot.Data.Entities.SteamAppPriceHistoryEntity", "SteamAppPrice")
                        .WithMany("TelegramNotifications")
                        .HasForeignKey("SteamAppPriceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SteamTelegramBot.Data.Entities.UserAppTrackingEntity", "UserAppTracking")
                        .WithMany("TelegramNotifications")
                        .HasForeignKey("UserAppTrackingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SteamAppPrice");

                    b.Navigation("UserAppTracking");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.UserAppTrackingEntity", b =>
                {
                    b.HasOne("SteamTelegramBot.Data.Entities.SteamAppEntity", "SteamApp")
                        .WithMany("TrackedUsers")
                        .HasForeignKey("SteamAppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SteamTelegramBot.Data.Entities.UserEntity", "User")
                        .WithMany("TrackedApps")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SteamApp");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.SteamAppEntity", b =>
                {
                    b.Navigation("PriceHistory");

                    b.Navigation("TrackedUsers");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.SteamAppPriceHistoryEntity", b =>
                {
                    b.Navigation("TelegramNotifications");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.UserAppTrackingEntity", b =>
                {
                    b.Navigation("TelegramNotifications");
                });

            modelBuilder.Entity("SteamTelegramBot.Data.Entities.UserEntity", b =>
                {
                    b.Navigation("TrackedApps");
                });
#pragma warning restore 612, 618
        }
    }
}
