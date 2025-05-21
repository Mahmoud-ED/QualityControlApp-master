using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class prog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e04a268-2296-47dc-b190-9f4149eeef36");

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("e36e5069-8b66-4c92-a685-cde57ea30f68"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("b76d2c4c-4679-4baf-8446-7c4e1fb0b432"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("3172a6a0-1b56-4a18-aeca-6536bed79aaf"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e5debec8-f206-400b-938e-9773f9d3bf8a", "a9088802-1303-4c8b-8bb7-003fb7d008b5", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5a69e756-046c-4d33-952e-582e1214d16b", 0, 0, null, "458378cf-a725-49df-8110-2f4c4fa2af46", new DateTime(2025, 5, 18, 16, 19, 7, 660, DateTimeKind.Utc).AddTicks(3228), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEO2J1kJm+E5sc6Gg2RQvZeTcFAp6E09ogQ2gZskMeIMueqdQKKEC+JM4VXoS7uCuVw==", null, true, "fbe9e28c-1dbb-4e53-bf1f-5bf7d41a7ac5", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("00abec82-d4c5-4b75-b137-0d828c0ab2f2"), new DateTime(2025, 5, 18, 18, 19, 7, 660, DateTimeKind.Local).AddTicks(2720), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("4ecddc22-09c3-40ac-acd5-8bd2d6e46dbb"), "", "LACC site", null, new DateTime(2025, 5, 18, 18, 19, 7, 660, DateTimeKind.Local).AddTicks(2492), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("9a2cc002-7349-4cc2-9bb6-a00cbe6323c7"), "The site is temporarily closed for development", new DateTime(2025, 5, 18, 18, 19, 7, 660, DateTimeKind.Local).AddTicks(2757), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e5debec8-f206-400b-938e-9773f9d3bf8a", "5a69e756-046c-4d33-952e-582e1214d16b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e5debec8-f206-400b-938e-9773f9d3bf8a", "5a69e756-046c-4d33-952e-582e1214d16b" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("00abec82-d4c5-4b75-b137-0d828c0ab2f2"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("4ecddc22-09c3-40ac-acd5-8bd2d6e46dbb"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("9a2cc002-7349-4cc2-9bb6-a00cbe6323c7"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5debec8-f206-400b-938e-9773f9d3bf8a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5a69e756-046c-4d33-952e-582e1214d16b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0e04a268-2296-47dc-b190-9f4149eeef36", "06462af7-da1e-404b-9d8d-811395e67ce5", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("e36e5069-8b66-4c92-a685-cde57ea30f68"), new DateTime(2025, 5, 18, 18, 9, 35, 570, DateTimeKind.Local).AddTicks(7006), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("b76d2c4c-4679-4baf-8446-7c4e1fb0b432"), "", "LACC site", null, new DateTime(2025, 5, 18, 18, 9, 35, 570, DateTimeKind.Local).AddTicks(6806), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("3172a6a0-1b56-4a18-aeca-6536bed79aaf"), "The site is temporarily closed for development", new DateTime(2025, 5, 18, 18, 9, 35, 570, DateTimeKind.Local).AddTicks(7054), null, true });
        }
    }
}
