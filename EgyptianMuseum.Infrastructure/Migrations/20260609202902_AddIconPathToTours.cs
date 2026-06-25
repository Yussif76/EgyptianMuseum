using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgyptianMuseum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIconPathToTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconPath",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconPath",
                table: "Tours");
        }
    }
}
