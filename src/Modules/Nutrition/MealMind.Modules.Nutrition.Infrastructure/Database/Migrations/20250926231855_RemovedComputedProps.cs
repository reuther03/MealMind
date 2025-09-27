using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedComputedProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PopularityScore",
                schema: "nutrition",
                table: "FoodStatistics");

            migrationBuilder.DropColumn(
                name: "WeightedRating",
                schema: "nutrition",
                table: "FoodStatistics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PopularityScore",
                schema: "nutrition",
                table: "FoodStatistics",
                type: "double precision",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeightedRating",
                schema: "nutrition",
                table: "FoodStatistics",
                type: "double precision",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
