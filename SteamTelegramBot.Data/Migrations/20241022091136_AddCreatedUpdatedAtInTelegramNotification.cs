using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamTelegramBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedUpdatedAtInTelegramNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TelegramNotifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TelegramNotifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TelegramNotifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TelegramNotifications");
        }
    }
}
