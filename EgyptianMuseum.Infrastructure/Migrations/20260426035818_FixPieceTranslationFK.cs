using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPieceTranslationFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PieceTranslations_Pieces_PieceId",
                table: "PieceTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_PieceTranslations_pieces_PiecesId",
                table: "PieceTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannedArtifacts_Pieces_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.DropIndex(
                name: "IX_PieceTranslations_PieceId",
                table: "PieceTranslations");

            migrationBuilder.DropIndex(
                name: "IX_PieceTranslations_PiecesId",
                table: "PieceTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pieces",
                table: "pieces");

            migrationBuilder.DropColumn(
                name: "PiecesId",
                table: "PieceTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "LanguageCode",
                table: "PieceTranslations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Artifactpieces",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PieceTranslations_PieceId_LanguageCode",
                table: "PieceTranslations",
                columns: new[] { "PieceId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artifactpieces_Code",
                table: "Artifactpieces",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PieceTranslations_Artifactpieces_PieceId",
                table: "PieceTranslations",
                column: "PieceId",
                principalTable: "Artifactpieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_PieceTranslations_Artifactpieces_PieceId",
                table: "PieceTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannedArtifacts_Artifactpieces_PieceId",
                table: "ScannedArtifacts");

            migrationBuilder.DropIndex(
                name: "IX_PieceTranslations_PieceId_LanguageCode",
                table: "PieceTranslations");

            migrationBuilder.DropIndex(
                name: "IX_Artifactpieces_Code",
                table: "Artifactpieces");


            migrationBuilder.AlterColumn<string>(
                name: "LanguageCode",
                table: "PieceTranslations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "PiecesId",
                table: "PieceTranslations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "pieces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pieces",
                table: "pieces",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PieceTranslations_PieceId",
                table: "PieceTranslations",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceTranslations_PiecesId",
                table: "PieceTranslations",
                column: "PiecesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PieceTranslations_Pieces_PieceId",
                table: "PieceTranslations",
                column: "PieceId",
                principalTable: "Pieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PieceTranslations_pieces_PiecesId",
                table: "PieceTranslations",
                column: "PiecesId",
                principalTable: "pieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannedArtifacts_Pieces_PieceId",
                table: "ScannedArtifacts",
                column: "PieceId",
                principalTable: "Pieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
