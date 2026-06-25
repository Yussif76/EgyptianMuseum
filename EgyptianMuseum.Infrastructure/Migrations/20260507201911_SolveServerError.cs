using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SolveServerError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourRooms_Rooms_RoomId1",
                table: "TourRooms");

            migrationBuilder.DropIndex(
                name: "IX_TourRooms_RoomId1",
                table: "TourRooms");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "TourRooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Artifactpieces",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artifactpieces_RoomId",
                table: "Artifactpieces",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artifactpieces_Rooms_RoomId",
                table: "Artifactpieces",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artifactpieces_Rooms_RoomId",
                table: "Artifactpieces");

            migrationBuilder.DropIndex(
                name: "IX_Artifactpieces_RoomId",
                table: "Artifactpieces");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Artifactpieces");

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "TourRooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourRooms_RoomId1",
                table: "TourRooms",
                column: "RoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TourRooms_Rooms_RoomId1",
                table: "TourRooms",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
