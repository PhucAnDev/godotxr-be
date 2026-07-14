using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalyzeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "CompletedExercises",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "LastAnalyzedAt",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "TotalExercises",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "TotalPracticeTime",
                table: "Analyzes");

            migrationBuilder.RenameColumn(
                name: "ProgressLevel",
                table: "Analyzes",
                newName: "SpeechLevel");

            migrationBuilder.RenameColumn(
                name: "PeriodStart",
                table: "Analyzes",
                newName: "NextAssessmentDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentDate",
                table: "Analyzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AttentionLevel",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommunicationAbility",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulties",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterventionGoals",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageComprehension",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageExpression",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PronunciationAbility",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialInteraction",
                table: "Analyzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 0, 0, 15, 447, DateTimeKind.Utc).AddTicks(9292));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 0, 0, 15, 447, DateTimeKind.Utc).AddTicks(9296));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 27, 0, 0, 15, 447, DateTimeKind.Utc).AddTicks(9297));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentDate",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "AttentionLevel",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "CommunicationAbility",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "Difficulties",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "InterventionGoals",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "LanguageComprehension",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "LanguageExpression",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "PronunciationAbility",
                table: "Analyzes");

            migrationBuilder.DropColumn(
                name: "SocialInteraction",
                table: "Analyzes");

            migrationBuilder.RenameColumn(
                name: "SpeechLevel",
                table: "Analyzes",
                newName: "ProgressLevel");

            migrationBuilder.RenameColumn(
                name: "NextAssessmentDate",
                table: "Analyzes",
                newName: "PeriodStart");

            migrationBuilder.AddColumn<float>(
                name: "AverageScore",
                table: "Analyzes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CompletedExercises",
                table: "Analyzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAnalyzedAt",
                table: "Analyzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Analyzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalExercises",
                table: "Analyzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPracticeTime",
                table: "Analyzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 26, 2, 32, 4, 18, DateTimeKind.Utc).AddTicks(4436));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 26, 2, 32, 4, 18, DateTimeKind.Utc).AddTicks(4440));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 26, 2, 32, 4, 18, DateTimeKind.Utc).AddTicks(4441));
        }
    }
}
