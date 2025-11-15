using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsSubscriptionTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionTier",
                schema: "nutrition",
                table: "UserProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionTier",
                schema: "nutrition",
                table: "UserProfile");
        }
    }
}
