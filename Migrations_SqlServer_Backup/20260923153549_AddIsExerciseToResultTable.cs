using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsExerciseToResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExercise",
                table: "Results",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "SpeechErrorCategory",
                table: "ChildSpeechAccuracies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Không",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "Thay thế âm");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 23, 22, 35, 46, 373, DateTimeKind.Utc).AddTicks(3672));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 23, 22, 35, 46, 373, DateTimeKind.Utc).AddTicks(3686));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 23, 22, 35, 46, 373, DateTimeKind.Utc).AddTicks(3688));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExercise",
                table: "Results");

            migrationBuilder.AlterColumn<string>(
                name: "SpeechErrorCategory",
                table: "ChildSpeechAccuracies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Thay thế âm",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "Không");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 17, 23, 2, 38, 584, DateTimeKind.Utc).AddTicks(690));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 17, 23, 2, 38, 584, DateTimeKind.Utc).AddTicks(710));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 9, 17, 23, 2, 38, 584, DateTimeKind.Utc).AddTicks(714));
        }
    }
}
