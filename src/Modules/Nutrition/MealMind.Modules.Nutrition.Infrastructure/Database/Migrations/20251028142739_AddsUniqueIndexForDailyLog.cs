using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsUniqueIndexForDailyLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DailyLog_UserId_CurrentDate",
                schema: "nutrition",
                table: "DailyLog",
                columns: new[] { "UserId", "CurrentDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyLog_UserId_CurrentDate",
                schema: "nutrition",
                table: "DailyLog");
        }
    }
}
