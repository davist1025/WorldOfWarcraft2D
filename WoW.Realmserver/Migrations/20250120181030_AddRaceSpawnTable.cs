using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceSpawnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_race_spawn",
                columns: table => new
                {
                    race_id = table.Column<int>(type: "int", nullable: false),
                    map_id = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    x_position = table.Column<float>(type: "float", nullable: false),
                    y_position = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_race_spawn", x => x.race_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_race_spawn");
        }
    }
}
