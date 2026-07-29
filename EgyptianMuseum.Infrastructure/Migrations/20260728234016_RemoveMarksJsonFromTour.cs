using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    public partial class RemoveMarksJsonFromTour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarksJson",
                table: "Tours");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarksJson",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }
    }
}