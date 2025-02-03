using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBehaviorsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "behavior_type",
                table: "npc_behavior");

            migrationBuilder.RenameColumn(
                name: "script",
                table: "npc_behavior",
                newName: "behavior_code_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "behavior_code_id",
                table: "npc_behavior",
                newName: "script");

            migrationBuilder.AddColumn<int>(
                name: "behavior_type",
                table: "npc_behavior",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
