using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangesTotalQuantityInGramesToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantityInGrams",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalQuantityInGrams",
                schema: "aichat",
                table: "ImageAnalyze",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
