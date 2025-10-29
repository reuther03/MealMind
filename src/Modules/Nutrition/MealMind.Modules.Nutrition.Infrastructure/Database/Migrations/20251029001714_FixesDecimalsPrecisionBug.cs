using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixesDecimalsPrecisionBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CaloriesGoal",
                schema: "nutrition",
                table: "DailyLog",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CaloriesGoal",
                schema: "nutrition",
                table: "DailyLog",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);
        }
    }
}
