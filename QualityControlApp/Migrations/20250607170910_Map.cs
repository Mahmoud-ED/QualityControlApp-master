using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class Map : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Map",
                table: "Location",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Map",
                table: "Location");
        }
    }
}
