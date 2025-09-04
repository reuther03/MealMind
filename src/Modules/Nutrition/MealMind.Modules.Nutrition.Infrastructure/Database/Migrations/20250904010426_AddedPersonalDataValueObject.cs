using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonalDataValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                schema: "nutrition",
                table: "UserProfile",
                newName: "PersonalData_DateOfBirth");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PersonalData_DateOfBirth",
                schema: "nutrition",
                table: "UserProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalData_ActivityLevel",
                schema: "nutrition",
                table: "UserProfile",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalData_ActivityLevel",
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

            migrationBuilder.RenameColumn(
                name: "PersonalData_DateOfBirth",
                schema: "nutrition",
                table: "UserProfile",
                newName: "DateOfBirth");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "nutrition",
                table: "UserProfile",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
