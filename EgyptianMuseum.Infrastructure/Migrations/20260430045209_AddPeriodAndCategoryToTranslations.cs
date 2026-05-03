using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodAndCategoryToTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannedArtifacts_Artifactpieces_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.DropTable(
                name: "Pieces");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PieceTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "PieceTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannedArtifacts_Artifactpieces_PieceId",
                table: "ScannedArtifacts",
                column: "PieceId",
                principalTable: "Artifactpieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannedArtifacts_Artifactpieces_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PieceTranslations");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "PieceTranslations");

            migrationBuilder.CreateTable(
                name: "Pieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabelText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pieces", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ScannedArtifacts_Artifactpieces_PieceId",
                table: "ScannedArtifacts",
                column: "PieceId",
                principalTable: "Pieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
