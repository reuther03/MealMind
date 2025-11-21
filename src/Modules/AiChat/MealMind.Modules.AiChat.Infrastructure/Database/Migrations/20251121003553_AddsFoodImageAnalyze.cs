using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsFoodImageAnalyze : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyImageAnalysisLimit",
                schema: "aichat",
                table: "AiChatUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoodImageAnalyze",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    CaloriesMin = table.Column<decimal>(type: "numeric", nullable: false),
                    CaloriesMax = table.Column<decimal>(type: "numeric", nullable: false),
                    ProteinMin = table.Column<decimal>(type: "numeric", nullable: false),
                    ProteinMax = table.Column<decimal>(type: "numeric", nullable: false),
                    CarbsMin = table.Column<decimal>(type: "numeric", nullable: false),
                    CarbsMax = table.Column<decimal>(type: "numeric", nullable: false),
                    FatMin = table.Column<decimal>(type: "numeric", nullable: false),
                    FatMax = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSugars = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalSaturatedFats = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalFiber = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalSodium = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalSalt = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalCholesterol = table.Column<decimal>(type: "numeric", nullable: true),
                    Response = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodImageAnalyze", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodImageAnalyze",
                schema: "aichat");

            migrationBuilder.DropColumn(
                name: "DailyImageAnalysisLimit",
                schema: "aichat",
                table: "AiChatUsers");
        }
    }
}
