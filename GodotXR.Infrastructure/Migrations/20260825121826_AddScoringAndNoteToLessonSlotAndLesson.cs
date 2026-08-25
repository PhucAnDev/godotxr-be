using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScoringAndNoteToLessonSlotAndLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CorrectPoints",
                table: "LessonSlots",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "WrongPoints",
                table: "LessonSlots",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CompletionBonusPoints",
                table: "Lessons",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 25, 19, 18, 22, 946, DateTimeKind.Utc).AddTicks(7987));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 25, 19, 18, 22, 946, DateTimeKind.Utc).AddTicks(8005));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 25, 19, 18, 22, 946, DateTimeKind.Utc).AddTicks(8009));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectPoints",
                table: "LessonSlots");

            migrationBuilder.DropColumn(
                name: "WrongPoints",
                table: "LessonSlots");

            migrationBuilder.DropColumn(
                name: "CompletionBonusPoints",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Lessons");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 15, 19, 18, 19, 26, DateTimeKind.Utc).AddTicks(8039));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 15, 19, 18, 19, 26, DateTimeKind.Utc).AddTicks(8050));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 15, 19, 18, 19, 26, DateTimeKind.Utc).AddTicks(8053));
        }
    }
}
