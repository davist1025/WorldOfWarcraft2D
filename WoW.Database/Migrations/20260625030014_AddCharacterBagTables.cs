using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterBagTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_bag_index",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    bag_slot_index = table.Column<int>(type: "int", nullable: false),
                    bag_item_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_bag_inventory",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "int", nullable: false),
                    bag_slot_index = table.Column<int>(type: "int", nullable: false),
                    bag_space_index = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    item_stack_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_bag_index");

            migrationBuilder.DropTable(
                name: "character_bag_inventory");
        }
    }
}
