using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class ConvertRequestStatusToEnumWithDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
         

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandingTakeoffTime",
                table: "AirPortRequests");

          
     
        }
    }
}
