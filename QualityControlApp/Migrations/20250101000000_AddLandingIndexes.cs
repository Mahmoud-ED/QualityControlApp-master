using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLandingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add indexes for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_Landings_OperatorName",
                table: "Landings",
                column: "OperatorName");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_AircraftRegistration",
                table: "Landings",
                column: "AircraftRegistration");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_DateOfFlights",
                table: "Landings",
                column: "DateOfFlights");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_RequestStatus",
                table: "Landings",
                column: "RequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_Created",
                table: "Landings",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_Email",
                table: "Landings",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_AircraftType",
                table: "Landings",
                column: "AircraftType");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_Route",
                table: "Landings",
                column: "Route");

            migrationBuilder.CreateIndex(
                name: "IX_Landings_AirportOfLanding",
                table: "Landings",
                column: "AirportOfLanding");

            // Composite indexes for common query patterns
            migrationBuilder.CreateIndex(
                name: "IX_Landings_Status_Date",
                table: "Landings",
                columns: new[] { "RequestStatus", "DateOfFlights" });

            migrationBuilder.CreateIndex(
                name: "IX_Landings_Operator_Date",
                table: "Landings",
                columns: new[] { "OperatorName", "DateOfFlights" });

            migrationBuilder.CreateIndex(
                name: "IX_Landings_Created_Status",
                table: "Landings",
                columns: new[] { "Created", "RequestStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes in reverse order
            migrationBuilder.DropIndex(
                name: "IX_Landings_Created_Status",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_Operator_Date",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_Status_Date",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_AirportOfLanding",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_Route",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_AircraftType",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_Email",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_Created",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_RequestStatus",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_DateOfFlights",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_AircraftRegistration",
                table: "Landings");

            migrationBuilder.DropIndex(
                name: "IX_Landings_OperatorName",
                table: "Landings");
        }
    }
}
