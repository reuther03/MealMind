using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixesImageAnalyzeSession2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropColumn(
                name: "ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyzeSessions_ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions",
                column: "ImageAnalyzeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyzeSessions_ImageAnalyze_ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions",
                column: "ImageAnalyzeId",
                principalSchema: "aichat",
                principalTable: "ImageAnalyze",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyzeSessions_ImageAnalyze_ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions");

            migrationBuilder.DropIndex(
                name: "IX_ImageAnalyzeSessions_ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions");

            migrationBuilder.DropColumn(
                name: "ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "ImageAnalyzeSessionId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_ImageAnalyzeSessionId1",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "ImageAnalyzeSessionId1",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id");
        }
    }
}
