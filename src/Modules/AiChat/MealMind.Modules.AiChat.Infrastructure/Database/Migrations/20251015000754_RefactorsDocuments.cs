using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace MealMind.Modules.AiChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorsDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RagDocument",
                schema: "aichat");

            migrationBuilder.CreateTable(
                name: "Documents",
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
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationDocuments",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationDocuments_Documents_Id",
                        column: x => x.Id,
                        principalSchema: "aichat",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationDocuments",
                schema: "aichat");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "aichat");

            migrationBuilder.CreateTable(
                name: "RagDocument",
                schema: "aichat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(768)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagDocument", x => x.Id);
                });
        }
    }
}
