using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Services.Outbox.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsEnumToStringConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                schema: "outbox",
                table: "OutboxEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                schema: "outbox",
                table: "OutboxEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
