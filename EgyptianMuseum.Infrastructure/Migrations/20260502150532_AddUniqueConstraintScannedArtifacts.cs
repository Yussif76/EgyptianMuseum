using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintScannedArtifacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScannedArtifacts_UserId",
                table: "ScannedArtifacts");

            migrationBuilder.CreateIndex(
                name: "UK_ScannedArtifacts_UserId_PieceId",
                table: "ScannedArtifacts",
                columns: new[] { "UserId", "PieceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_ScannedArtifacts_UserId_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.CreateIndex(
                name: "IX_ScannedArtifacts_UserId",
                table: "ScannedArtifacts",
                column: "UserId");
        }
    }
}
