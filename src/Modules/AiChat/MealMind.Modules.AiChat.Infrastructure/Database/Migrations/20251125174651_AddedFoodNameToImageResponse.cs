using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedFoodNameToImageResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodName",
                schema: "aichat",
                table: "ImageAnalyze");
        }
    }
}
