using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixesImageAnalyzeSession3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropColumn(
                name: "ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyze_SessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_SessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "SessionId",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_SessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropIndex(
                name: "IX_ImageAnalyze_SessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "ImageAnalyzeSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "ImageAnalyzeSessionId",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
