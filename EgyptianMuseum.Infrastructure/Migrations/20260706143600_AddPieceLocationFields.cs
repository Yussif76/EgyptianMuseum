using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPieceLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Collection",
                table: "Artifactpieces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GalleryNum",
                table: "Artifactpieces",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PieceLocationJson",
                table: "Artifactpieces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collection",
                table: "Artifactpieces");

            migrationBuilder.DropColumn(
                name: "GalleryNum",
                table: "Artifactpieces");

            migrationBuilder.DropColumn(
                name: "PieceLocationJson",
                table: "Artifactpieces");
        }
    }
}
