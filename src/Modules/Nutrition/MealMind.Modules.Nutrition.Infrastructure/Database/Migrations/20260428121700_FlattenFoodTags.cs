using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class FlattenFoodTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodCategories",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodDietaryTags",
                schema: "nutrition");

            migrationBuilder.AddColumn<string[]>(
                name: "_categories",
                schema: "nutrition",
                table: "Food",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "_dietaryTags",
                schema: "nutrition",
                table: "Food",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_categories",
                schema: "nutrition",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "_dietaryTags",
                schema: "nutrition",
                table: "Food");

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodCategories_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodDietaryTags",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DietaryTag = table.Column<string>(type: "text", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodDietaryTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodDietaryTags_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_FoodId_Category",
                schema: "nutrition",
                table: "FoodCategories",
                columns: new[] { "FoodId", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodDietaryTags_FoodId_DietaryTag",
                schema: "nutrition",
                table: "FoodDietaryTags",
                columns: new[] { "FoodId", "DietaryTag" },
                unique: true);
        }
    }
}
