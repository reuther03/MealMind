using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Services.Outbox.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixedNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "outbox");

            migrationBuilder.RenameTable(
                name: "OutboxEvents",
                schema: "__OutboxDbContext",
                newName: "OutboxEvents",
                newSchema: "outbox");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "__OutboxDbContext");

            migrationBuilder.RenameTable(
                name: "OutboxEvents",
                schema: "outbox",
                newName: "OutboxEvents",
                newSchema: "__OutboxDbContext");
        }
    }
}
