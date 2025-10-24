using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsNewProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanExportData",
                schema: "aichat",
                table: "AiChatUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUseAdvancedPrompts",
                schema: "aichat",
                table: "AiChatUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ConversationsMessagesHistoryDaysLimit",
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
                name: "CanExportData",
                schema: "aichat",
                table: "AiChatUsers");

            migrationBuilder.DropColumn(
                name: "CanUseAdvancedPrompts",
                schema: "aichat",
                table: "AiChatUsers");

            migrationBuilder.DropColumn(
                name: "ConversationsMessagesHistoryDaysLimit",
                schema: "aichat",
                table: "AiChatUsers");
        }
    }
}
