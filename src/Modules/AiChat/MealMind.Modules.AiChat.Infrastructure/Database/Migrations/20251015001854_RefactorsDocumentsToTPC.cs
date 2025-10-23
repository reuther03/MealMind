using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorsDocumentsToTPC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationDocuments_Documents_Id",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachedAt",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ChunkIndex",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentGroupId",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "vector(768)",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "aichat",
                table: "ConversationDocuments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RagDocuments",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(768)", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    DocumentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagDocuments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RagDocuments",
                schema: "aichat");

            migrationBuilder.DropColumn(
                name: "AttachedAt",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "ChunkIndex",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "Content",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentGroupId",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "Embedding",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "aichat",
                table: "ConversationDocuments");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationDocuments_Documents_Id",
                schema: "aichat",
                table: "ConversationDocuments",
                column: "Id",
                principalSchema: "aichat",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
