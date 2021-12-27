using Microsoft.EntityFrameworkCore.Migrations;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    public partial class Topics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PassiveReplyOptions");

            migrationBuilder.DropColumn(
                name: "IsDirectMessage",
                table: "PassiveReplyOptions");

            migrationBuilder.DropColumn(
                name: "IsDota",
                table: "PassiveReplyOptions");

            migrationBuilder.DropColumn(
                name: "IsMeme",
                table: "PassiveReplyOptions");

            migrationBuilder.CreateTable(
                name: "PassiveTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    Regex = table.Column<string>(type: "TEXT", unicode: false, nullable: true),
                    IsDirectMessage = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsMeme = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsExclusive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Probability = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassiveTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PassiveReplyOptionPassiveTopic",
                columns: table => new
                {
                    PassiveTopicsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReplyOptionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassiveReplyOptionPassiveTopic", x => new { x.PassiveTopicsId, x.ReplyOptionsId });
                    table.ForeignKey(
                        name: "FK_PassiveReplyOptionPassiveTopic_PassiveReplyOptions_ReplyOptionsId",
                        column: x => x.ReplyOptionsId,
                        principalTable: "PassiveReplyOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassiveReplyOptionPassiveTopic_PassiveTopics_PassiveTopicsId",
                        column: x => x.PassiveTopicsId,
                        principalTable: "PassiveTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PassiveReplyOptionPassiveTopic_ReplyOptionsId",
                table: "PassiveReplyOptionPassiveTopic",
                column: "ReplyOptionsId");

            migrationBuilder.InsertData(
                table: "PassiveTopics",
                new[] { "Name", "Regex", "IsDirectMessage", "IsMeme", "IsExclusive", "Probability" },
                new object[,]
                {
                    {
                        "regular", null, null, null, false, 250
                    },
                    {
                        "direct", null, true, null, false, 3
                    },
                    {
                        "meme", null, null, true, true, 15
                    },
                    {
                        "dota", @"(\W|^)((дот)|(дотк)|(дотан))(а|е|у|ы)?(\W|$)", null, false, true, 4
                    },
                }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PassiveReplyOptionPassiveTopic");

            migrationBuilder.DropTable(
                name: "PassiveTopics");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsMeme",
                table: "PassiveReplyOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
