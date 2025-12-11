using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsImageAnalyzeSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageAnalyzeSessions",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnalyzeSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "ImageAnalyzeSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_Id",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "Id",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_Id",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropTable(
                name: "ImageAnalyzeSessions",
                schema: "aichat");

            migrationBuilder.DropIndex(
                name: "IX_ImageAnalyze_ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropColumn(
                name: "ImageAnalyzeSessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
