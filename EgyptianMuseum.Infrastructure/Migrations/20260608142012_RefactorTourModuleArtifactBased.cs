using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTourModuleArtifactBased : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Tours",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarksJson",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PathImageUrl",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TourPieces",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "int", nullable: false),
                    PieceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPieces", x => new { x.TourId, x.PieceId });
                    table.ForeignKey(
                        name: "FK_TourPieces_Artifactpieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Artifactpieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourPieces_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourTranslations_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourPieces_PieceId",
                table: "TourPieces",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPieces_TourId_PieceId",
                table: "TourPieces",
                columns: new[] { "TourId", "PieceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslations_TourId_LanguageCode",
                table: "TourTranslations",
                columns: new[] { "TourId", "LanguageCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourPieces");

            migrationBuilder.DropTable(
                name: "TourTranslations");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "MarksJson",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PathImageUrl",
                table: "Tours");
        }
    }
}
