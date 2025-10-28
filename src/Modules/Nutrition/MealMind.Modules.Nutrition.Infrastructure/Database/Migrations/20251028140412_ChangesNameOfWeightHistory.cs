using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangesNameOfWeightHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeightHistories_UserProfile_UserProfileId",
                schema: "nutrition",
                table: "WeightHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeightHistories",
                schema: "nutrition",
                table: "WeightHistories");

            migrationBuilder.RenameTable(
                name: "WeightHistories",
                schema: "nutrition",
                newName: "WeightHistory",
                newSchema: "nutrition");

            migrationBuilder.RenameIndex(
                name: "IX_WeightHistories_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory",
                newName: "IX_WeightHistory_UserProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeightHistory",
                schema: "nutrition",
                table: "WeightHistory",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeightHistory_UserProfile_UserProfileId",
                schema: "nutrition",
                table: "WeightHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeightHistory",
                schema: "nutrition",
                table: "WeightHistory");

            migrationBuilder.RenameTable(
                name: "WeightHistory",
                schema: "nutrition",
                newName: "WeightHistories",
                newSchema: "nutrition");

            migrationBuilder.RenameIndex(
                name: "IX_WeightHistory_UserProfileId",
                schema: "nutrition",
                table: "WeightHistories",
                newName: "IX_WeightHistories_UserProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeightHistories",
                schema: "nutrition",
                table: "WeightHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeightHistories_UserProfile_UserProfileId",
                schema: "nutrition",
                table: "WeightHistories",
                column: "UserProfileId",
                principalSchema: "nutrition",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
