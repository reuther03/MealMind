using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredToFoodStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFoodData",
                schema: "nutrition");

            migrationBuilder.CreateTable(
                name: "FoodStatistics",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalUsageCount = table.Column<int>(type: "integer", nullable: false),
                    FavoriteCount = table.Column<int>(type: "integer", nullable: false),
                    AverageRating = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: false),
                    RatingCount = table.Column<int>(type: "integer", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SearchCount = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodStatistics_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodStatistics_FoodId",
                schema: "nutrition",
                table: "FoodStatistics",
                column: "FoodId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodStatistics",
                schema: "nutrition");

            migrationBuilder.CreateTable(
                name: "UserFoodData",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    TimesUsed = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFoodData_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodData_FoodId",
                schema: "nutrition",
                table: "UserFoodData",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodData_UserId_FoodId",
                schema: "nutrition",
                table: "UserFoodData",
                columns: new[] { "UserId", "FoodId" },
                unique: true);
        }
    }
}
