using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsSubscriptionTierWithConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubscriptionTier",
                schema: "nutrition",
                table: "UserProfile",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionTier",
                schema: "nutrition",
                table: "UserProfile",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
