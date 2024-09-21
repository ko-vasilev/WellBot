using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MessageLogSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "MessageLogs",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "MessageLogs");
        }
    }
}
