using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MessageLogOptOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "MessageLogs",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.CreateTable(
                name: "MessageLogOptOuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLogOptOuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageLogOptOuts_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogOptOuts_ChatId",
                table: "MessageLogOptOuts",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageLogOptOuts");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "MessageLogs",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);
        }
    }
}
