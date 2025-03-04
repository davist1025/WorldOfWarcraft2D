using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterAndNPCStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "agility_amt",
                table: "npc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "intellect_amt",
                table: "npc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "spirit_amt",
                table: "npc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stamina_amt",
                table: "npc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "strength_amt",
                table: "npc",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "character_class_stats",
                columns: table => new
                {
                    class_id = table.Column<int>(type: "int", nullable: false),
                    strength_amt = table.Column<int>(type: "int", nullable: false),
                    agility_amt = table.Column<int>(type: "int", nullable: false),
                    intellect_amt = table.Column<int>(type: "int", nullable: false),
                    stamina_amt = table.Column<int>(type: "int", nullable: false),
                    spirit_amt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_class_stats", x => x.class_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_stats",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "int", nullable: false),
                    strength_amt = table.Column<int>(type: "int", nullable: false),
                    agility_amt = table.Column<int>(type: "int", nullable: false),
                    intellect_amt = table.Column<int>(type: "int", nullable: false),
                    stamina_amt = table.Column<int>(type: "int", nullable: false),
                    spirit_amt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_stats", x => x.character_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_class_stats");

            migrationBuilder.DropTable(
                name: "character_stats");

            migrationBuilder.DropColumn(
                name: "agility_amt",
                table: "npc");

            migrationBuilder.DropColumn(
                name: "intellect_amt",
                table: "npc");

            migrationBuilder.DropColumn(
                name: "spirit_amt",
                table: "npc");

            migrationBuilder.DropColumn(
                name: "stamina_amt",
                table: "npc");

            migrationBuilder.DropColumn(
                name: "strength_amt",
                table: "npc");
        }
    }
}
