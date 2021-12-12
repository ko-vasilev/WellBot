using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class PidorMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalTelegramUserName",
                table: "PidorRegistrations",
                type: "TEXT",
                unicode: false,
                maxLength: 63,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedMessageId",
                table: "ChatPidors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PidorResultMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageRaw = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    GameDay = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PidorResultMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatPidors_UsedMessageId",
                table: "ChatPidors",
                column: "UsedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPidors_PidorResultMessages_UsedMessageId",
                table: "ChatPidors",
                column: "UsedMessageId",
                principalTable: "PidorResultMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPidors_PidorResultMessages_UsedMessageId",
                table: "ChatPidors");

            migrationBuilder.DropTable(
                name: "PidorResultMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatPidors_UsedMessageId",
                table: "ChatPidors");

            migrationBuilder.DropColumn(
                name: "OriginalTelegramUserName",
                table: "PidorRegistrations");

            migrationBuilder.DropColumn(
                name: "UsedMessageId",
                table: "ChatPidors");
        }
    }
}
