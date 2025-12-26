using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangesMealTypeToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First convert string values to integers
            migrationBuilder.Sql("""
                UPDATE nutrition."Meal"
                SET "MealType" = CASE "MealType"
                    WHEN 'Breakfast' THEN '0'
                    WHEN 'Lunch' THEN '1'
                    WHEN 'Dinner' THEN '2'
                    WHEN 'Snack' THEN '3'
                    WHEN 'Other' THEN '4'
                    ELSE '0'
                END
                """);

            // Then alter column type with USING clause
            migrationBuilder.Sql("""
                ALTER TABLE nutrition."Meal"
                ALTER COLUMN "MealType" TYPE integer
                USING "MealType"::integer
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert back to text
            migrationBuilder.Sql("""
                ALTER TABLE nutrition."Meal"
                ALTER COLUMN "MealType" TYPE text
                USING "MealType"::text
                """);

            // Convert integers back to string names
            migrationBuilder.Sql("""
                UPDATE nutrition."Meal"
                SET "MealType" = CASE "MealType"
                    WHEN '0' THEN 'Breakfast'
                    WHEN '1' THEN 'Lunch'
                    WHEN '2' THEN 'Dinner'
                    WHEN '3' THEN 'Snack'
                    WHEN '4' THEN 'Other'
                    ELSE 'Breakfast'
                END
                """);
        }
    }
}
