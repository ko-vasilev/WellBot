using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "SlapOptions",
                type: "TEXT",
                unicode: false,
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MessageRaw",
                table: "PidorResultMessages",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalTelegramUserName",
                table: "PidorRegistrations",
                type: "TEXT",
                unicode: false,
                maxLength: 63,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 63,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PassiveTopics",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PassiveReplyOptions",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ChatDatas",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AutomaticMessageTemplates",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CronInterval",
                table: "AutomaticMessageTemplates",
                type: "TEXT",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "SlapOptions",
                type: "TEXT",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "MessageRaw",
                table: "PidorResultMessages",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalTelegramUserName",
                table: "PidorRegistrations",
                type: "TEXT",
                unicode: false,
                maxLength: 63,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 63);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PassiveTopics",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PassiveReplyOptions",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "ChatDatas",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AutomaticMessageTemplates",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "CronInterval",
                table: "AutomaticMessageTemplates",
                type: "TEXT",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false);
        }
    }
}
