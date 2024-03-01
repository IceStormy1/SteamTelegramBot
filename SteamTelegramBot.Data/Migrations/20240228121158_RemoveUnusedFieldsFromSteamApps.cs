using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamTelegramBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedFieldsFromSteamApps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "SteamApps");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "SteamApps");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SteamApps");

            migrationBuilder.AddColumn<long>(
                name: "TelegramChatId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramChatId",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "SteamApps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "SteamApps",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SteamApps",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
