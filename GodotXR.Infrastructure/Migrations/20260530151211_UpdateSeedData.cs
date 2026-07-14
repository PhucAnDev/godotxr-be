using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 15, 12, 10, 735, DateTimeKind.Utc).AddTicks(5246));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "RoleName" },
                values: new object[] { new DateTime(2026, 5, 30, 15, 12, 10, 735, DateTimeKind.Utc).AddTicks(5255), "Teacher", "Teacher" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 15, 12, 10, 735, DateTimeKind.Utc).AddTicks(5256));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "IsDeleted", "RoleName", "UpdatedAt" },
                values: new object[] { 4, new DateTime(2026, 5, 30, 15, 12, 10, 735, DateTimeKind.Utc).AddTicks(5257), "Child", true, false, "Child", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Username" },
                values: new object[] { "teacher@godotxr.com", "teacher" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 31, 22, 146, DateTimeKind.Utc).AddTicks(8046));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "RoleName" },
                values: new object[] { new DateTime(2026, 5, 29, 7, 31, 22, 146, DateTimeKind.Utc).AddTicks(8050), "Lecture", "Lecture" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 31, 22, 146, DateTimeKind.Utc).AddTicks(8052));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Username" },
                values: new object[] { "lecture@godotxr.com", "lecture" });
        }
    }
}
