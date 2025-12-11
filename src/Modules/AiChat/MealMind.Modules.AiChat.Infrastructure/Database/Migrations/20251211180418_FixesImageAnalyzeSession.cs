using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixesImageAnalyzeSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_Id",
                schema: "aichat",
                table: "ImageAnalyze");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_Id",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "Id",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
