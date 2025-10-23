using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredToProperTPC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents",
                schema: "aichat");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "aichat",
                table: "RagDocuments",
                type: "vector(768)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(768)");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "vector(768)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(768)");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RagDocuments_DocumentGroupId_ChunkIndex",
                schema: "aichat",
                table: "RagDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RagDocuments_Embedding",
                schema: "aichat",
                table: "RagDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ConversationDocuments_ConversationId_DocumentGroupId_ChunkI~",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ConversationDocuments_Embedding",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "aichat",
                table: "RagDocuments",
                type: "vector(768)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(768)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "vector(768)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(768)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    DocumentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(768)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });
        }
    }
}
