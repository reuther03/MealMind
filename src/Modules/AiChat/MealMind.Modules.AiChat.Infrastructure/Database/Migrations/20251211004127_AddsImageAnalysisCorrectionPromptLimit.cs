using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsImageAnalysisCorrectionPromptLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageAnalysisCorrectionPromptLimit",
                schema: "aichat",
                table: "AiChatUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAnalysisCorrectionPromptLimit",
                schema: "aichat",
                table: "AiChatUsers");
        }
    }
}
