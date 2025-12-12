using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "aichat");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "AiChatUsers",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<string>(type: "text", nullable: false),
                    ActiveConversations = table.Column<int>(type: "integer", nullable: false),
                    ConversationsLimit = table.Column<int>(type: "integer", nullable: false),
                    ConversationsMessagesHistoryDaysLimit = table.Column<int>(type: "integer", nullable: false),
                    DocumentsLimit = table.Column<int>(type: "integer", nullable: false),
                    PromptTokensLimit = table.Column<int>(type: "integer", nullable: false),
                    ResponseTokensLimit = table.Column<int>(type: "integer", nullable: false),
                    DailyPromptsLimit = table.Column<int>(type: "integer", nullable: false),
                    CanExportData = table.Column<bool>(type: "boolean", nullable: false),
                    CanUseAdvancedPrompts = table.Column<bool>(type: "boolean", nullable: false),
                    DailyImageAnalysisLimit = table.Column<int>(type: "integer", nullable: false),
                    ImageAnalysisCorrectionPromptLimit = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiChatUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatConversations",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatConversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationDocuments",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1024)", nullable: true),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    DocumentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RagDocuments",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1024)", nullable: true),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    DocumentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ReplyToMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatConversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "aichat",
                        principalTable: "ChatConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageAnalyze",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodName = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Prompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    CaloriesMin = table.Column<decimal>(type: "numeric", nullable: false),
                    CaloriesMax = table.Column<decimal>(type: "numeric", nullable: false),
                    ProteinMin = table.Column<decimal>(type: "numeric", nullable: false),
                    ProteinMax = table.Column<decimal>(type: "numeric", nullable: false),
                    CarbsMin = table.Column<decimal>(type: "numeric", nullable: false),
                    CarbsMax = table.Column<decimal>(type: "numeric", nullable: false),
                    FatMin = table.Column<decimal>(type: "numeric", nullable: false),
                    FatMax = table.Column<decimal>(type: "numeric", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalQuantityInGrams = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnalyze", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageAnalyzeSessions",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageAnalyzeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnalyzeSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageAnalyzeSessions_ImageAnalyze_ImageAnalyzeId",
                        column: x => x.ImageAnalyzeId,
                        principalSchema: "aichat",
                        principalTable: "ImageAnalyze",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversations_UserId_LastUsedAt",
                schema: "aichat",
                table: "ChatConversations",
                columns: new[] { "UserId", "LastUsedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId_CreatedAt",
                schema: "aichat",
                table: "ChatMessages",
                columns: new[] { "ConversationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationDocuments_ConversationId_DocumentGroupId_ChunkI~",
                schema: "aichat",
                table: "ConversationDocuments",
                columns: new[] { "ConversationId", "DocumentGroupId", "ChunkIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationDocuments_Embedding",
                schema: "aichat",
                table: "ConversationDocuments",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyze_SessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnalyzeSessions_ImageAnalyzeId",
                schema: "aichat",
                table: "ImageAnalyzeSessions",
                column: "ImageAnalyzeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RagDocuments_DocumentGroupId_ChunkIndex",
                schema: "aichat",
                table: "RagDocuments",
                columns: new[] { "DocumentGroupId", "ChunkIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_RagDocuments_Embedding",
                schema: "aichat",
                table: "RagDocuments",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_SessionId",
                schema: "aichat",
                table: "ImageAnalyze",
                column: "SessionId",
                principalSchema: "aichat",
                principalTable: "ImageAnalyzeSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnalyze_ImageAnalyzeSessions_SessionId",
                schema: "aichat",
                table: "ImageAnalyze");

            migrationBuilder.DropTable(
                name: "AiChatUsers",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "ChatMessages",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "ConversationDocuments",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "RagDocuments",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "ChatConversations",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "ImageAnalyzeSessions",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "ImageAnalyze",
                schema: "aichat");
        }
    }
}
