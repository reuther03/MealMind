using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsSubscriptionValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                schema: "identity",
                table: "IdentityUsers");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                schema: "identity",
                table: "IdentityUsers");

            migrationBuilder.DropColumn(
                name: "Tier",
                schema: "identity",
                table: "IdentityUsers");

            migrationBuilder.CreateTable(
                name: "Subscription",
                schema: "identity",
                columns: table => new
                {
                    IdentityUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<string>(type: "text", nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    StripeSubscriptionId = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SubscriptionStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubscriptionStatus = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.IdentityUserId);
                    table.ForeignKey(
                        name: "FK_Subscription_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalSchema: "identity",
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscription",
                schema: "identity");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                schema: "identity",
                table: "IdentityUsers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                schema: "identity",
                table: "IdentityUsers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tier",
                schema: "identity",
                table: "IdentityUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
