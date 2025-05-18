using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFileType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "21891ccc-a394-4941-b65d-2ed718d3cf85", "df1f4687-14e8-44b1-87cb-44bd6431cf97" });

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("2ea77a1c-574e-4952-98eb-a52bab8e9210"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("5e503a8b-a58f-49db-b0e5-bafb5b945e93"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("8833c216-3cc3-4e14-86e6-54d82e558789"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21891ccc-a394-4941-b65d-2ed718d3cf85");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1f4687-14e8-44b1-87cb-44bd6431cf97");

            migrationBuilder.AddColumn<Guid>(
                name: "FileTypeId",
                table: "AirPortRequestFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "FileType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileType", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AirPortRequestFiles_FileTypeId",
                table: "AirPortRequestFiles",
                column: "FileTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AirPortRequestFiles_FileType_FileTypeId",
                table: "AirPortRequestFiles",
                column: "FileTypeId",
                principalTable: "FileType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPortRequestFiles_FileType_FileTypeId",
                table: "AirPortRequestFiles");

            migrationBuilder.DropTable(
                name: "FileType");

            migrationBuilder.DropIndex(
                name: "IX_AirPortRequestFiles_FileTypeId",
                table: "AirPortRequestFiles");

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

            migrationBuilder.DropColumn(
                name: "FileTypeId",
                table: "AirPortRequestFiles");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "21891ccc-a394-4941-b65d-2ed718d3cf85", "1cce3105-7442-48df-aa47-e35ad3a85045", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "Approval", "ConcurrencyStamp", "CreatedDate", "Discriminator", "Email", "EmailConfirmed", "LastAccessTime", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "df1f4687-14e8-44b1-87cb-44bd6431cf97", 0, 0, null, "dcf93eb9-826a-49ca-af40-ec82a0a3b647", new DateTime(2025, 4, 22, 9, 47, 49, 80, DateTimeKind.Utc).AddTicks(1174), "ApplicationUser", "libyanlacc@gmail.com", true, null, true, null, null, "LIBYANLACC@GMAIL.COM", "LIBYANLACC@GMAIL.COM", "AQAAAAIAAYagAAAAEISgRox1jfnsqRAzkAsZA7vJxfUMItP+2YU2qkOJKjHFaXElFbcMBSovxUOiBvZ+Mw==", null, true, "ede5bbfd-f4bc-42bc-bec3-9d27bd05c8ca", false, "libyanlacc@gmail.com" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("2ea77a1c-574e-4952-98eb-a52bab8e9210"), new DateTime(2025, 4, 22, 11, 47, 49, 80, DateTimeKind.Local).AddTicks(881), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("5e503a8b-a58f-49db-b0e5-bafb5b945e93"), "", "LACC site", null, new DateTime(2025, 4, 22, 11, 47, 49, 80, DateTimeKind.Local).AddTicks(623), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("8833c216-3cc3-4e14-86e6-54d82e558789"), "The site is temporarily closed for development", new DateTime(2025, 4, 22, 11, 47, 49, 80, DateTimeKind.Local).AddTicks(915), null, true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "21891ccc-a394-4941-b65d-2ed718d3cf85", "df1f4687-14e8-44b1-87cb-44bd6431cf97" });
        }
    }
}
