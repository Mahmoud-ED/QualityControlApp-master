using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCulmnEmailOnLanding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Landing",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "178c19d1-471c-42f5-b652-8b4d6170bd04");

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("6c3a3d28-a71d-4157-a5c6-df2b0b2c5210"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("c60e3012-3eb4-49f0-9b67-547757beca32"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("5e56f722-70b2-4e1e-a858-6a2907370fb4"));

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Landing");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7be7b76f-4f30-41f7-b2b7-ab73ca52b3ae", "46c84eac-793c-4640-94c0-a93b13bff79e", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("d091af05-50e4-41ae-99f4-6d009722d527"), new DateTime(2025, 5, 12, 19, 46, 22, 560, DateTimeKind.Local).AddTicks(7225), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("29625343-130f-4881-b44c-926da87d3eaf"), "", "LACC site", null, new DateTime(2025, 5, 12, 19, 46, 22, 560, DateTimeKind.Local).AddTicks(7008), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("6ed2cff1-a76f-4b7d-85c5-d78d70f47b74"), "The site is temporarily closed for development", new DateTime(2025, 5, 12, 19, 46, 22, 560, DateTimeKind.Local).AddTicks(7255), null, true });
        }
    }
}
