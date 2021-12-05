using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class PidorReg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PidorRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PidorRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PidorRegistrations_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PidorRegistrations_ChatId",
                table: "PidorRegistrations",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_PidorRegistrations_TelegramUserId",
                table: "PidorRegistrations",
                column: "TelegramUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PidorRegistrations");
        }
    }
}
