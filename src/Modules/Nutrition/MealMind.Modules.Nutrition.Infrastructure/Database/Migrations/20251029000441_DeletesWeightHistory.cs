using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class DeletesWeightHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeightHistory",
                schema: "nutrition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeightHistory",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightHistory", x => x.Id);
                });
        }
    }
}
