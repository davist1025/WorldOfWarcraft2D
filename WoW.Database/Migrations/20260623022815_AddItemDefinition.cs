using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "character_name_filter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    phrase = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_name_filter", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    character_name = table.Column<string>(type: "varchar(16)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    character_race_id = table.Column<int>(type: "int", nullable: false),
                    character_hair_id = table.Column<int>(type: "int", nullable: false),
                    zone_x_position = table.Column<float>(type: "float", nullable: false),
                    zone_y_position = table.Column<float>(type: "float", nullable: false),
                    map_id = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    animation_direction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.character_name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chat_command",
                columns: table => new
                {
                    command_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    help_text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    security_level = table.Column<int>(type: "int", nullable: false),
                    handler_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_command", x => x.command_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chat_command_children",
                columns: table => new
                {
                    child_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    help_text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    security_level = table.Column<int>(type: "int", nullable: false),
                    handler_id = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_command_children", x => x.child_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inventory_modelid = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rendered_modelid = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    itemtype_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "npc",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    model_id = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    npc_flag = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    strength_amt = table.Column<int>(type: "int", nullable: true),
                    agility_amt = table.Column<int>(type: "int", nullable: true),
                    intellect_amt = table.Column<int>(type: "int", nullable: true),
                    stamina_amt = table.Column<int>(type: "int", nullable: true),
                    spirit_amt = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npc", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "npc_behavior",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    npc_id = table.Column<int>(type: "int", nullable: false),
                    behavior_code_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npc_behavior", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_class_stats");

            migrationBuilder.DropTable(
                name: "character_name_filter");

            migrationBuilder.DropTable(
                name: "character_race_spawn");

            migrationBuilder.DropTable(
                name: "character_stats");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "chat_command");

            migrationBuilder.DropTable(
                name: "chat_command_children");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "npc");

            migrationBuilder.DropTable(
                name: "npc_behavior");
        }
    }
}
