using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveCollectionToPieceTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collection",
                table: "Artifactpieces");

            migrationBuilder.AddColumn<string>(
                name: "Collection",
                table: "PieceTranslations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collection",
                table: "PieceTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Collection",
                table: "Artifactpieces",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
