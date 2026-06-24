using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMind.Modules.Training.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddsNavPropToSessionExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SessionExercise_ExerciseId",
                schema: "training",
                table: "SessionExercise",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercise_Exercise_ExerciseId",
                schema: "training",
                table: "SessionExercise",
                column: "ExerciseId",
                principalSchema: "training",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercise_Exercise_ExerciseId",
                schema: "training",
                table: "SessionExercise");

            migrationBuilder.DropIndex(
                name: "IX_SessionExercise_ExerciseId",
                schema: "training",
                table: "SessionExercise");
        }
    }
}
