using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewQuastion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5ef73333-4e7d-4400-b343-6c8971e66e77", "ae0e8c60-105a-4086-96ca-108b562ced63" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("07d809bd-3821-4981-b4ea-a51b7fe20ad3"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("50b27fad-4288-46cd-a57c-08bdd772037d"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("60bd2b0e-6a09-4b60-8031-707d35afa1f5"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ef73333-4e7d-4400-b343-6c8971e66e77");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ae0e8c60-105a-4086-96ca-108b562ced63");

            migrationBuilder.AddColumn<string>(
                name: "Inspect",
                table: "CompanyQuestionContent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "CompanyQuestionContent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nots",
                table: "CompanyQuestionContent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CompanyQuestion",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Inspect",
                table: "CompanyQuestionContent");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "CompanyQuestionContent");

            migrationBuilder.DropColumn(
                name: "Nots",
                table: "CompanyQuestionContent");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CompanyQuestion");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5ef73333-4e7d-4400-b343-6c8971e66e77", "6242958f-e430-4f1e-a842-38b2e29308eb", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ae0e8c60-105a-4086-96ca-108b562ced63", 0, 0, null, "790fdab7-5516-4f9a-9ba9-ef834158e346", new DateTime(2025, 4, 22, 12, 40, 26, 264, DateTimeKind.Utc).AddTicks(2142), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEIjphxY5vVGB4UIPs6Ftas8BuTXH4mmcz9jm1KXyluDKMhGmFg4N6G7AAftulvKw6Q==", null, true, "4dd9e23d-892a-4068-8b13-1077f486df99", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("07d809bd-3821-4981-b4ea-a51b7fe20ad3"), new DateTime(2025, 4, 22, 14, 40, 26, 264, DateTimeKind.Local).AddTicks(1805), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("50b27fad-4288-46cd-a57c-08bdd772037d"), "", "LACC site", null, new DateTime(2025, 4, 22, 14, 40, 26, 264, DateTimeKind.Local).AddTicks(1541), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("60bd2b0e-6a09-4b60-8031-707d35afa1f5"), "The site is temporarily closed for development", new DateTime(2025, 4, 22, 14, 40, 26, 264, DateTimeKind.Local).AddTicks(1877), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5ef73333-4e7d-4400-b343-6c8971e66e77", "ae0e8c60-105a-4086-96ca-108b562ced63" });
        }
    }
}
