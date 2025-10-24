using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsAiChatUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiChatUsers",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<string>(type: "text", nullable: false),
                    ActiveConversations = table.Column<int>(type: "integer", nullable: false),
                    ConversationsLimit = table.Column<int>(type: "integer", nullable: false),
                    DocumentsLimit = table.Column<int>(type: "integer", nullable: false),
                    PromptTokensLimit = table.Column<int>(type: "integer", nullable: false),
                    ResponseTokensLimit = table.Column<int>(type: "integer", nullable: false),
                    DailyPromptsLimit = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiChatUsers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiChatUsers",
                schema: "aichat");
        }
    }
}
