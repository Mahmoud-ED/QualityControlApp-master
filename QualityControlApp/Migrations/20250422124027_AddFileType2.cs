using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFileType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "18f63ac2-ca08-41ca-9dea-6c4e12fbbc89", "4b99c074-018e-4f27-a4dd-e02b372cc66f" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("f16cc6be-98cb-4882-a162-59565024cd1d"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("379724f2-6862-498f-8db5-2eba771e045d"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("61f49d47-b74f-4a94-8225-0095004cb93c"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18f63ac2-ca08-41ca-9dea-6c4e12fbbc89");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4b99c074-018e-4f27-a4dd-e02b372cc66f");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FileType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FileType",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "18f63ac2-ca08-41ca-9dea-6c4e12fbbc89", "846548de-987a-4181-83e1-45e082c2e921", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4b99c074-018e-4f27-a4dd-e02b372cc66f", 0, 0, null, "9bf6e6b2-7fd8-43c5-9e17-439a80e50481", new DateTime(2025, 4, 22, 11, 9, 26, 41, DateTimeKind.Utc).AddTicks(2690), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEHihgOYde8JRJZiAwXUGg18x/2xGtWUXAsB8lmkoVOg+TAQoJjsPvc597HWg2BbLYg==", null, true, "de196ae2-e36b-4c2c-a44b-e2e00ee9c23a", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("f16cc6be-98cb-4882-a162-59565024cd1d"), new DateTime(2025, 4, 22, 13, 9, 26, 41, DateTimeKind.Local).AddTicks(2394), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("379724f2-6862-498f-8db5-2eba771e045d"), "", "LACC site", null, new DateTime(2025, 4, 22, 13, 9, 26, 41, DateTimeKind.Local).AddTicks(2201), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("61f49d47-b74f-4a94-8225-0095004cb93c"), "The site is temporarily closed for development", new DateTime(2025, 4, 22, 13, 9, 26, 41, DateTimeKind.Local).AddTicks(2423), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "18f63ac2-ca08-41ca-9dea-6c4e12fbbc89", "4b99c074-018e-4f27-a4dd-e02b372cc66f" });
        }
    }
}
