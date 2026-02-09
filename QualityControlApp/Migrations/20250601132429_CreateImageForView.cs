using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateImageForView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImgeForViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ViewName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgeForViews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImgeForViews");
        }
    }
}
