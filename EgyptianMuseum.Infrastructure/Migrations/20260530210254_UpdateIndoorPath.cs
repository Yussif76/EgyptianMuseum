using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndoorPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "FromRoom",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "FromX",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "FromY",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "ToRoom",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "ToX",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "ToY",
                table: "IndoorMapPaths");

            migrationBuilder.AddColumn<int>(
                name: "FromRoomId",
                table: "IndoorMapPaths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToRoomId",
                table: "IndoorMapPaths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IndoorMapPaths_FromRoomId",
                table: "IndoorMapPaths",
                column: "FromRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_IndoorMapPaths_ToRoomId",
                table: "IndoorMapPaths",
                column: "ToRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_IndoorMapPaths_Rooms_FromRoomId",
                table: "IndoorMapPaths",
                column: "FromRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IndoorMapPaths_Rooms_ToRoomId",
                table: "IndoorMapPaths",
                column: "ToRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndoorMapPaths_Rooms_FromRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.DropForeignKey(
                name: "FK_IndoorMapPaths_Rooms_ToRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.DropIndex(
                name: "IX_IndoorMapPaths_FromRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.DropIndex(
                name: "IX_IndoorMapPaths_ToRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "FromRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "ToRoomId",
                table: "IndoorMapPaths");

            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "IndoorMapPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "FromRoom",
                table: "IndoorMapPaths",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "FromX",
                table: "IndoorMapPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FromY",
                table: "IndoorMapPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ToRoom",
                table: "IndoorMapPaths",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ToX",
                table: "IndoorMapPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToY",
                table: "IndoorMapPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
