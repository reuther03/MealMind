using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPropToFoodEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Source",
                schema: "nutrition",
                table: "Food",
                newName: "FoodDataSource");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                schema: "nutrition",
                table: "FoodEntry",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "nutrition",
                table: "FoodEntry",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                schema: "nutrition",
                table: "FoodEntry");

            migrationBuilder.RenameColumn(
                name: "FoodDataSource",
                schema: "nutrition",
                table: "Food",
                newName: "Source");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodId",
                schema: "nutrition",
                table: "FoodEntry",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
