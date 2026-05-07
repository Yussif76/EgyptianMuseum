using Microsoft.EntityFrameworkCore.Migrations;

namespace EgyptianMuseum.Infrastructure.Migrations
{
    public partial class AddRoomNamesToIndoorMapPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromRoom",
                table: "IndoorMapPaths",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToRoom",
                table: "IndoorMapPaths",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromRoom",
                table: "IndoorMapPaths");

            migrationBuilder.DropColumn(
                name: "ToRoom",
                table: "IndoorMapPaths");
        }
    }
}
