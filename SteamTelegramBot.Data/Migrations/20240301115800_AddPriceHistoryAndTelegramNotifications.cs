using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SteamTelegramBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceHistoryAndTelegramNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "SteamApps");

            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "SteamApps");

            migrationBuilder.CreateTable(
                name: "SteamAppPriceHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SteamAppId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceType = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamAppPriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SteamAppPriceHistory_SteamApps_SteamAppId",
                        column: x => x.SteamAppId,
                        principalTable: "SteamApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserAppTrackingId = table.Column<long>(type: "bigint", nullable: false),
                    SteamAppPriceId = table.Column<long>(type: "bigint", nullable: false),
                    WasSent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramNotifications_SteamAppPriceHistory_SteamAppPriceId",
                        column: x => x.SteamAppPriceId,
                        principalTable: "SteamAppPriceHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramNotifications_UserTrackedApps_UserAppTrackingId",
                        column: x => x.UserAppTrackingId,
                        principalTable: "UserTrackedApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SteamAppPriceHistory_SteamAppId_Version",
                table: "SteamAppPriceHistory",
                columns: new[] { "SteamAppId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramNotifications_SteamAppPriceId",
                table: "TelegramNotifications",
                column: "SteamAppPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramNotifications_UserAppTrackingId",
                table: "TelegramNotifications",
                column: "UserAppTrackingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramNotifications");

            migrationBuilder.DropTable(
                name: "SteamAppPriceHistory");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "SteamApps",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceType",
                table: "SteamApps",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
