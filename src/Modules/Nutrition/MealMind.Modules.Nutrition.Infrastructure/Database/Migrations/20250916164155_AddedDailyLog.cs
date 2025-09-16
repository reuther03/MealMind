using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedDailyLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DailyLogId",
                schema: "nutrition",
                table: "Meal",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyLog",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CaloriesGoal = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meal_DailyLogId",
                schema: "nutrition",
                table: "Meal",
                column: "DailyLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_DailyLog_DailyLogId",
                schema: "nutrition",
                table: "Meal",
                column: "DailyLogId",
                principalSchema: "nutrition",
                principalTable: "DailyLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meal_DailyLog_DailyLogId",
                schema: "nutrition",
                table: "Meal");

            migrationBuilder.DropTable(
                name: "DailyLog",
                schema: "nutrition");

            migrationBuilder.DropIndex(
                name: "IX_Meal_DailyLogId",
                schema: "nutrition",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "DailyLogId",
                schema: "nutrition",
                table: "Meal");
        }
    }
}
