using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class ChatDataUserMention : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasUserMention",
                table: "ChatDatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
UPDATE ChatDatas
SET     HasUserMention = 1
WHERE   Text Like '%@%'
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasUserMention",
                table: "ChatDatas");
        }
    }
}
