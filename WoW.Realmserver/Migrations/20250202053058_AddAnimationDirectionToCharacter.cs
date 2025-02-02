using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimationDirectionToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "animation_direction",
                table: "characters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "animation_direction",
                table: "characters");
        }
    }
}
