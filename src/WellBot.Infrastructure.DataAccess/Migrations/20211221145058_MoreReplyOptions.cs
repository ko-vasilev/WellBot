using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class MoreReplyOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDirectMessage",
                table: "PassiveReplyOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDota",
                table: "PassiveReplyOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDirectMessage",
                table: "PassiveReplyOptions");

            migrationBuilder.DropColumn(
                name: "IsDota",
                table: "PassiveReplyOptions");
        }
    }
}
