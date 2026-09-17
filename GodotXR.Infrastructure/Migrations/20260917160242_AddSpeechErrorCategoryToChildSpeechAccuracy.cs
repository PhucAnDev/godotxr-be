using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeechErrorCategoryToChildSpeechAccuracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpeechErrorCategory",
                table: "ChildSpeechAccuracies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Thay thế âm");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpeechErrorCategory",
                table: "ChildSpeechAccuracies");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 21, 57, 31, 824, DateTimeKind.Utc).AddTicks(5122));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 21, 57, 31, 824, DateTimeKind.Utc).AddTicks(5145));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 21, 57, 31, 824, DateTimeKind.Utc).AddTicks(5147));
        }
    }
}
