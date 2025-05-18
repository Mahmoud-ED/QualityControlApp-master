using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToCategoryTypeQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e79ca8f9-59f2-4deb-a33b-7bf9af0c6b5f", "35abda20-cc9a-472b-ac89-52ab5477c0e8" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("f582ace7-4d81-4059-bd55-9152a34167e2"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("acb43541-44c3-4708-867b-307352855fd6"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("5a1391f1-5f87-4fef-8b64-347f13b1e594"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e79ca8f9-59f2-4deb-a33b-7bf9af0c6b5f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "35abda20-cc9a-472b-ac89-52ab5477c0e8");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "QuestionCategoryType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "69213899-2851-4a75-9154-3c0e72415ea4", "049a59d8-6601-4338-bc59-0eec23e0dd2a", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "56bcceb0-c7a6-49c0-9136-27be057f417e", 0, 0, null, "fd8b0aa2-c08f-496b-b6fe-2e0b9e56a6e0", new DateTime(2025, 4, 25, 9, 56, 2, 75, DateTimeKind.Utc).AddTicks(3445), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEJG3//9Ep2zLoh5AC7Em+BVdnc0GLTX6xL35zk2W57Uq+xy4FjXX8VsoWPeerKvGQw==", null, true, "4be4c876-41e4-47a9-87e2-eae7a0516c25", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("81082f25-7427-487b-b732-e878fe26135c"), new DateTime(2025, 4, 25, 11, 56, 2, 75, DateTimeKind.Local).AddTicks(3166), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("7e7d1abe-dbf4-4850-aa49-ebde2460ac70"), "", "LACC site", null, new DateTime(2025, 4, 25, 11, 56, 2, 75, DateTimeKind.Local).AddTicks(2976), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("571a9185-9baa-4f84-9458-e3cda07359ea"), "The site is temporarily closed for development", new DateTime(2025, 4, 25, 11, 56, 2, 75, DateTimeKind.Local).AddTicks(3195), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "69213899-2851-4a75-9154-3c0e72415ea4", "56bcceb0-c7a6-49c0-9136-27be057f417e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "69213899-2851-4a75-9154-3c0e72415ea4", "56bcceb0-c7a6-49c0-9136-27be057f417e" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("81082f25-7427-487b-b732-e878fe26135c"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("7e7d1abe-dbf4-4850-aa49-ebde2460ac70"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("571a9185-9baa-4f84-9458-e3cda07359ea"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "69213899-2851-4a75-9154-3c0e72415ea4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "56bcceb0-c7a6-49c0-9136-27be057f417e");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "QuestionCategoryType");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e79ca8f9-59f2-4deb-a33b-7bf9af0c6b5f", "c4ea1eb8-8554-4d72-8309-8bb32badc990", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "35abda20-cc9a-472b-ac89-52ab5477c0e8", 0, 0, null, "951400a7-1764-4689-9ba3-718109f8d114", new DateTime(2025, 4, 24, 19, 59, 5, 496, DateTimeKind.Utc).AddTicks(6244), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEPkBA1RAOCc98+cU9EqiX6Ro8L7Dt1odUv3lkwlo7vmN+uE5GNEhqNyyH84XhABHag==", null, true, "46036f57-a2ec-4a73-9811-9b51f6c80043", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("f582ace7-4d81-4059-bd55-9152a34167e2"), new DateTime(2025, 4, 24, 21, 59, 5, 496, DateTimeKind.Local).AddTicks(6014), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("acb43541-44c3-4708-867b-307352855fd6"), "", "LACC site", null, new DateTime(2025, 4, 24, 21, 59, 5, 496, DateTimeKind.Local).AddTicks(5841), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("5a1391f1-5f87-4fef-8b64-347f13b1e594"), "The site is temporarily closed for development", new DateTime(2025, 4, 24, 21, 59, 5, 496, DateTimeKind.Local).AddTicks(6037), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e79ca8f9-59f2-4deb-a33b-7bf9af0c6b5f", "35abda20-cc9a-472b-ac89-52ab5477c0e8" });
        }
    }
}
