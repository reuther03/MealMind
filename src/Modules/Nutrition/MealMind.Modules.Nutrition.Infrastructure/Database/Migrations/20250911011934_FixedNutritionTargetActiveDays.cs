using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixedNutritionTargetActiveDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_NutritionTargetActiveDays_NutritionTarget_NutritionTargetId",
                schema: "nutrition",
                table: "NutritionTargetActiveDays",
                column: "NutritionTargetId",
                principalSchema: "nutrition",
                principalTable: "NutritionTarget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NutritionTargetActiveDays_NutritionTarget_NutritionTargetId",
                schema: "nutrition",
                table: "NutritionTargetActiveDays");
        }
    }
}
