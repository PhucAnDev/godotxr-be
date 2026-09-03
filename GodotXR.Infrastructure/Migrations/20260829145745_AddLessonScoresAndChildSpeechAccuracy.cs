using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodotXR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonScoresAndChildSpeechAccuracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CorrectAnswerScore",
                table: "Lessons",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "IncorrectAnswerScore",
                table: "Lessons",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "ChildSpeechAccuracies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildProfileId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    LessonSlotId = table.Column<int>(type: "int", nullable: true),
                    ResultId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Word = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccuracyScore = table.Column<float>(type: "real", nullable: false),
                    FluencyScore = table.Column<float>(type: "real", nullable: true),
                    PronunciationScore = table.Column<float>(type: "real", nullable: true),
                    CompletenessScore = table.Column<float>(type: "real", nullable: true),
                    ErrorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AudioChunkIndex = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildSpeechAccuracies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildSpeechAccuracies_ChildProfiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "ChildProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildSpeechAccuracies_LessonSlots_LessonSlotId",
                        column: x => x.LessonSlotId,
                        principalTable: "LessonSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildSpeechAccuracies_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildSpeechAccuracies_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ChildSpeechAccuracies_ChildProfileId",
                table: "ChildSpeechAccuracies",
                column: "ChildProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildSpeechAccuracies_LessonId",
                table: "ChildSpeechAccuracies",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildSpeechAccuracies_LessonSlotId",
                table: "ChildSpeechAccuracies",
                column: "LessonSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildSpeechAccuracies_ResultId",
                table: "ChildSpeechAccuracies",
                column: "ResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildSpeechAccuracies");

            migrationBuilder.DropColumn(
                name: "CorrectAnswerScore",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "IncorrectAnswerScore",
                table: "Lessons");

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
    }
}
