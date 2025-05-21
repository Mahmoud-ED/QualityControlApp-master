using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "bbaabb43-181a-44fa-8470-96292aa1d203", "2e9a2d79-dd2b-4b10-9d0f-2a3b7fc2646e" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("2752fc40-cc2e-4126-9ba3-db0c3773da2b"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("2784e687-9b70-4534-9136-fbfc787e1041"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("ccca9fbe-0b3f-4d5b-9e6d-ac027b08bbf0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbaabb43-181a-44fa-8470-96292aa1d203");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e9a2d79-dd2b-4b10-9d0f-2a3b7fc2646e");

        

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
    }
}
