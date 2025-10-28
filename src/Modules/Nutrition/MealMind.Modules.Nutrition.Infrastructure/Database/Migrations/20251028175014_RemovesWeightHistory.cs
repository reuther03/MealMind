using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovesWeightHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeightHistory_UserProfile_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory");

            migrationBuilder.DropIndex(
                name: "IX_WeightHistory_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WeightHistory_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeightHistory_UserProfile_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory",
                column: "UserProfileId",
                principalSchema: "nutrition",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
