using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nutrition");

            migrationBuilder.CreateTable(
                name: "DailyLog",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CaloriesGoal = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Brand = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeightHistory",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meal",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MealType = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DailyLogId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meal_DailyLog_DailyLogId",
                        column: x => x.DailyLogId,
                        principalSchema: "nutrition",
                        principalTable: "DailyLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodCategories_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodDietaryTags",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    DietaryTag = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodDietaryTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodDietaryTags_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodReviews",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodReviews_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodStatistics",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalUsageCount = table.Column<int>(type: "integer", nullable: false),
                    FavoriteCount = table.Column<int>(type: "integer", nullable: false),
                    AverageRating = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: false),
                    RatingCount = table.Column<int>(type: "integer", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SearchCount = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodStatistics_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionPer100G",
                schema: "nutrition",
                columns: table => new
                {
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Calories = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    Protein = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Fat = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    SaturatedFat = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    Carbohydrates = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Sugar = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    Fiber = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    Sodium = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    Salt = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    Cholesterol = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionPer100G", x => x.FoodId);
                    table.ForeignKey(
                        name: "FK_NutritionPer100G_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteFoods",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteFoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteFoods_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "nutrition",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteMeals",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteMeals_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "nutrition",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionTarget",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Calories = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    ProteinGrams = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    CarbohydratesGrams = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    FatsGrams = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    WaterIntake = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionTarget_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "nutrition",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalData",
                schema: "nutrition",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Height = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    WeightTarget = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    ActivityLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalData", x => x.UserProfileId);
                    table.ForeignKey(
                        name: "FK_PersonalData_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "nutrition",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodEntry",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodName = table.Column<string>(type: "text", nullable: false),
                    FoodBrand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantityInGrams = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalCalories = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalProteins = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalCarbohydrates = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalSugars = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    TotalFats = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalSaturatedFats = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    TotalFiber = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    TotalSodium = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TotalSalt = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TotalCholesterol = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    MealId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodEntry_Meal_MealId",
                        column: x => x.MealId,
                        principalSchema: "nutrition",
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionTargetActiveDays",
                schema: "nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NutritionTargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionTargetActiveDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionTargetActiveDays_NutritionTarget_NutritionTargetId",
                        column: x => x.NutritionTargetId,
                        principalSchema: "nutrition",
                        principalTable: "NutritionTarget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyLog_UserId_CurrentDate",
                schema: "nutrition",
                table: "DailyLog",
                columns: new[] { "UserId", "CurrentDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteFoods_UserProfileId",
                schema: "nutrition",
                table: "FavoriteFoods",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMeals_UserProfileId",
                schema: "nutrition",
                table: "FavoriteMeals",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Food_Barcode",
                schema: "nutrition",
                table: "Food",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_FoodId_Category",
                schema: "nutrition",
                table: "FoodCategories",
                columns: new[] { "FoodId", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodDietaryTags_FoodId_DietaryTag",
                schema: "nutrition",
                table: "FoodDietaryTags",
                columns: new[] { "FoodId", "DietaryTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntry_MealId",
                schema: "nutrition",
                table: "FoodEntry",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReviews_FoodId_UserId",
                schema: "nutrition",
                table: "FoodReviews",
                columns: new[] { "FoodId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodStatistics_FoodId",
                schema: "nutrition",
                table: "FoodStatistics",
                column: "FoodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meal_DailyLogId",
                schema: "nutrition",
                table: "Meal",
                column: "DailyLogId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionTarget_UserProfileId",
                schema: "nutrition",
                table: "NutritionTarget",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionTargetActiveDays_NutritionTargetId_DayOfWeek",
                schema: "nutrition",
                table: "NutritionTargetActiveDays",
                columns: new[] { "NutritionTargetId", "DayOfWeek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteFoods",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FavoriteMeals",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodCategories",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodDietaryTags",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodEntry",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodReviews",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "FoodStatistics",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "NutritionPer100G",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "NutritionTargetActiveDays",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "PersonalData",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "WeightHistory",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "Meal",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "Food",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "NutritionTarget",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "DailyLog",
                schema: "nutrition");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "nutrition");
        }
    }
}
