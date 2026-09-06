using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WoW.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_stackable",
                table: "Items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_stackable",
                table: "Items");
        }
    }
}
