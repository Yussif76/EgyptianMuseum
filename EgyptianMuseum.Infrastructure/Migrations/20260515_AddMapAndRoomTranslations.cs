using Microsoft.EntityFrameworkCore.Migrations;

namespace EgyptianMuseum.Infrastructure.Migrations
{
    public partial class AddMapAndRoomTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create MapTranslation table
            migrationBuilder.CreateTable(
                name: "MapTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapTranslation_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create RoomTranslation table
            migrationBuilder.CreateTable(
                name: "RoomTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTranslation_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create unique indexes
            migrationBuilder.CreateIndex(
                name: "IX_MapTranslation_MapId_LanguageCode",
                table: "MapTranslation",
                columns: new[] { "MapId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomTranslation_RoomId_LanguageCode",
                table: "RoomTranslation",
                columns: new[] { "RoomId", "LanguageCode" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables
            migrationBuilder.DropTable(
                name: "MapTranslation");

            migrationBuilder.DropTable(
                name: "RoomTranslation");
        }
    }
}
