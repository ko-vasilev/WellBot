using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class AutomaticMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutomaticMessageTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    ImageSearchQuery = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    CronInterval = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    LastTriggeredDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticMessageTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomaticMessageTemplates_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticMessageTemplates_ChatId",
                table: "AutomaticMessageTemplates",
                column: "ChatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomaticMessageTemplates");
        }
    }
}
