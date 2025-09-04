using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredPersonalDataStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalData_ActivityLevel",
                schema: "nutrition",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "PersonalData_DateOfBirth",
                schema: "nutrition",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "PersonalData_Gender",
                schema: "nutrition",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "PersonalData_Height",
                schema: "nutrition",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "PersonalData_Weight",
                schema: "nutrition",
                table: "UserProfile");

            migrationBuilder.CreateTable(
                name: "UserProfile_PersonalData",
                schema: "nutrition",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    Height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ActivityLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile_PersonalData", x => x.UserProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfile_PersonalData_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "nutrition",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfile_PersonalData",
                schema: "nutrition");

            migrationBuilder.AddColumn<string>(
                name: "PersonalData_ActivityLevel",
                schema: "nutrition",
                table: "UserProfile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "PersonalData_DateOfBirth",
                schema: "nutrition",
                table: "UserProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "PersonalData_Gender",
                schema: "nutrition",
                table: "UserProfile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PersonalData_Height",
                schema: "nutrition",
                table: "UserProfile",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PersonalData_Weight",
                schema: "nutrition",
                table: "UserProfile",
                type: "numeric(6,3)",
                precision: 6,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
