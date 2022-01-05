using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class AudioTopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAudio",
                table: "PassiveTopics",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.InsertData(
                table: "PassiveTopics",
                new[] { "Name", "Regex", "IsDirectMessage", "IsMeme", "IsAudio", "IsExclusive", "Probability" },
                new object[,]
                {
                    {
                        "audio", null, null, null, true, true, 20
                    },
                }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAudio",
                table: "PassiveTopics");
            migrationBuilder.DeleteData(
                table: "PassiveTopics",
                "Name",
                "audio");
        }
    }
}
