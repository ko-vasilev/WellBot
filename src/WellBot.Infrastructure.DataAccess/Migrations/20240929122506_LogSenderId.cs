using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LogSenderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SenderTelegramId",
                table: "MessageLogs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderTelegramId",
                table: "MessageLogs");
        }
    }
}
