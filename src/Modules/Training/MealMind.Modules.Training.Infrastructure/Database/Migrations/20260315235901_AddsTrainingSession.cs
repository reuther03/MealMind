using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MealMind.Modules.Training.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsTrainingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "training",
                table: "TrainingSession",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                schema: "training",
                table: "TrainingSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                schema: "training",
                table: "TrainingSession",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SessionExercise",
                schema: "training",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CardioDetails_DurationInMinutes = table.Column<int>(type: "integer", nullable: true),
                    CardioDetails_DistanceInKm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    CardioDetails_CaloriesBurned = table.Column<int>(type: "integer", nullable: true),
                    CardioDetails_AverageHeartRate = table.Column<int>(type: "integer", nullable: true),
                    CardioDetails_AverageSpeed = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    CardioDetails_Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CardioDetails_CaloriesEstimated = table.Column<int>(type: "integer", nullable: true),
                    TrainingSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionExercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionExercise_TrainingSession_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalSchema: "training",
                        principalTable: "TrainingSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSet",
                schema: "training",
                columns: table => new
                {
                    SessionExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Repetitions = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    SetType = table.Column<string>(type: "text", nullable: false),
                    RestTimeInSeconds = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_SessionExercise_TrainingSessionId",
                schema: "training",
                table: "SessionExercise",
                column: "TrainingSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSet",
                schema: "training");

            migrationBuilder.DropTable(
                name: "SessionExercise",
                schema: "training");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "training",
                table: "TrainingSession");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                schema: "training",
                table: "TrainingSession");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                schema: "training",
                table: "TrainingSession");
        }
    }
}
