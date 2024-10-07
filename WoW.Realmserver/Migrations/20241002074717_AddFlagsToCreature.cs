using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsToCreature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAggressive",
                table: "creatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTargetable",
                table: "creatures",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAggressive",
                table: "creatures");

            migrationBuilder.DropColumn(
                name: "IsTargetable",
                table: "creatures");
        }
    }
}
