using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class Relation2 : Migration
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
                keyValues: new object[] { "071dbc1b-f453-4417-b8fc-a151cafb07a3", "698b899d-78e6-4a9f-8ce9-f21e23c570d1" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("403bed52-50ea-4fbe-beaa-86d91b5821fb"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("0ab0351e-0064-4a50-8bf8-76b5b18fa46a"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("5d335bb4-94a1-40c0-9991-90b8c9f53c7f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "071dbc1b-f453-4417-b8fc-a151cafb07a3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "698b899d-78e6-4a9f-8ce9-f21e23c570d1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bbaabb43-181a-44fa-8470-96292aa1d203", "c6ed10f5-3273-4d15-8e64-f67f6051d579", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2e9a2d79-dd2b-4b10-9d0f-2a3b7fc2646e", 0, 0, null, "140ff37b-71eb-4745-bb4c-6588eef0ca43", new DateTime(2025, 5, 19, 18, 35, 22, 988, DateTimeKind.Utc).AddTicks(9537), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAECo4Z4zl0RT9eaSwM+ttrf6K6obDb8xYPFyDmvS3UqO4m4kzGt2rqMhvcqoJFFIm4g==", null, true, "21d59da7-f75a-45eb-89e0-87f16241d66f", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("2752fc40-cc2e-4126-9ba3-db0c3773da2b"), new DateTime(2025, 5, 19, 20, 35, 22, 988, DateTimeKind.Local).AddTicks(9267), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("2784e687-9b70-4534-9136-fbfc787e1041"), "", "LACC site", null, new DateTime(2025, 5, 19, 20, 35, 22, 988, DateTimeKind.Local).AddTicks(9123), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("ccca9fbe-0b3f-4d5b-9e6d-ac027b08bbf0"), "The site is temporarily closed for development", new DateTime(2025, 5, 19, 20, 35, 22, 988, DateTimeKind.Local).AddTicks(9301), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "bbaabb43-181a-44fa-8470-96292aa1d203", "2e9a2d79-dd2b-4b10-9d0f-2a3b7fc2646e" });
        }
    }
}
