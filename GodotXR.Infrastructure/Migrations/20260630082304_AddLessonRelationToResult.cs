using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonRelationToResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ExerciseId",
                table: "Results",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "Results",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultType",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 30, 8, 23, 1, 14, DateTimeKind.Utc).AddTicks(2689));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 30, 8, 23, 1, 14, DateTimeKind.Utc).AddTicks(2706));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 30, 8, 23, 1, 14, DateTimeKind.Utc).AddTicks(2708));

            migrationBuilder.CreateIndex(
                name: "IX_Results_LessonId",
                table: "Results",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Lessons_LessonId",
                table: "Results",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Lessons_LessonId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_LessonId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "ResultType",
                table: "Results");

            migrationBuilder.AlterColumn<int>(
                name: "ExerciseId",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 5, 7, 56, 642, DateTimeKind.Utc).AddTicks(6718));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 5, 7, 56, 642, DateTimeKind.Utc).AddTicks(6723));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 5, 7, 56, 642, DateTimeKind.Utc).AddTicks(6725));
        }
    }
}
