using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedSaltAndChoresterolProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Salt",
                schema: "nutrition",
                table: "NutritionPer100G",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCholesterol",
                schema: "nutrition",
                table: "FoodEntry",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSalt",
                schema: "nutrition",
                table: "FoodEntry",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                schema: "nutrition",
                table: "NutritionPer100G");

            migrationBuilder.DropColumn(
                name: "TotalCholesterol",
                schema: "nutrition",
                table: "FoodEntry");

            migrationBuilder.DropColumn(
                name: "TotalSalt",
                schema: "nutrition",
                table: "FoodEntry");
        }
    }
}
