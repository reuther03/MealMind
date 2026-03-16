using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MealMind.Modules.Training.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangesDetailsToJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSet",
                schema: "training");

            migrationBuilder.DropColumn(
                name: "CardioDetails_AverageHeartRate",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_AverageSpeed",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_CaloriesBurned",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_CaloriesEstimated",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_DistanceInKm",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_DurationInMinutes",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "CardioDetails_Notes",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.AddColumn<string>(
                name: "CardioDetails",
                schema: "training",
                table: "SessionExercise",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrengthDetails",
                schema: "training",
                table: "SessionExercise",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardioDetails",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropColumn(
                name: "StrengthDetails",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.AddColumn<int>(
                name: "CardioDetails_AverageHeartRate",
                schema: "training",
                table: "SessionExercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CardioDetails_AverageSpeed",
                schema: "training",
                table: "SessionExercise",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardioDetails_CaloriesBurned",
                schema: "training",
                table: "SessionExercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardioDetails_CaloriesEstimated",
                schema: "training",
                table: "SessionExercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CardioDetails_DistanceInKm",
                schema: "training",
                table: "SessionExercise",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardioDetails_DurationInMinutes",
                schema: "training",
                table: "SessionExercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardioDetails_Notes",
                schema: "training",
                table: "SessionExercise",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExerciseSet",
                schema: "training",
                columns: table => new
                {
                    SessionExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Repetitions = table.Column<int>(type: "integer", nullable: false),
                    RestTimeInSeconds = table.Column<int>(type: "integer", nullable: true),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    SetType = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSet", x => new { x.SessionExerciseId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExerciseSet_SessionExercise_SessionExerciseId",
                        column: x => x.SessionExerciseId,
                        principalSchema: "training",
                        principalTable: "SessionExercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
