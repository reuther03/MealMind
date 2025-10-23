using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddsRelationshipToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatConversations_ConversationId",
                schema: "aichat",
                table: "ChatMessages",
                column: "ConversationId",
                principalSchema: "aichat",
                principalTable: "ChatConversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatConversations_ConversationId",
                schema: "aichat",
                table: "ChatMessages");
        }
    }
}
