using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredNutritionTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Protein",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "ProteinGrams");

            migrationBuilder.RenameColumn(
                name: "Fats",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "FatsGrams");

            migrationBuilder.RenameColumn(
                name: "Carbohydrates",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "CarbohydratesGrams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProteinGrams",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "Protein");

            migrationBuilder.RenameColumn(
                name: "FatsGrams",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "Fats");

            migrationBuilder.RenameColumn(
                name: "CarbohydratesGrams",
                schema: "nutrition",
                table: "NutritionTarget",
                newName: "Carbohydrates");
        }
    }
}
