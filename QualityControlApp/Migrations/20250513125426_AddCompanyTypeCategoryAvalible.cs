using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTypeCategoryAvalible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AlterColumn<string>(
                name: "AocDocumentPath",
                table: "Landing",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CompanyTypeCategoryAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CompanyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionCategoryTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTypeCategoryAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyTypeCategoryAvailable_CompanyType_CompanyTypeId",
                        column: x => x.CompanyTypeId,
                        principalTable: "CompanyType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyTypeCategoryAvailable_QuestionCategoryType_QuestionCategoryTypeId",
                        column: x => x.QuestionCategoryTypeId,
                        principalTable: "QuestionCategoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          
          

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTypeCategoryAvailable_CompanyTypeId",
                table: "CompanyTypeCategoryAvailable",
                column: "CompanyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTypeCategoryAvailable_QuestionCategoryTypeId",
                table: "CompanyTypeCategoryAvailable",
                column: "QuestionCategoryTypeId");

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPortRequestFiles_Landing_LandingId",
                table: "AirPortRequestFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Landing_AspNetUsers_ApproverUserId",
                table: "Landing");

            migrationBuilder.DropTable(
                name: "CompanyTypeCategoryAvailable");

            migrationBuilder.DropIndex(
                name: "IX_AirPortRequestFiles_LandingId",
                table: "AirPortRequestFiles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3c80870-3b59-4be9-898e-48d19c4c933d");

            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: new Guid("3a0f4266-cf4b-43f3-a784-2b92195d9e6b"));

            migrationBuilder.DeleteData(
                table: "SiteInfo",
                keyColumn: "Id",
                keyValue: new Guid("e07775bf-b676-4147-92a4-f7a79f7280ca"));

            migrationBuilder.DeleteData(
                table: "SiteState",
                keyColumn: "Id",
                keyValue: new Guid("587e8ffd-2d66-4a8c-8ede-26786c4d4811"));

            migrationBuilder.AlterColumn<string>(
                name: "AocDocumentPath",
                table: "Landing",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "178c19d1-471c-42f5-b652-8b4d6170bd04", "c7e71d60-4a3e-4381-9ecb-d1c78da61a1c", "Prog", "PROG" });

            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "Created", "Email", "Facebook", "Instagram", "Modified", "Phone", "Twitter" },
                values: new object[] { new Guid("6c3a3d28-a71d-4157-a5c6-df2b0b2c5210"), new DateTime(2025, 5, 12, 22, 48, 35, 146, DateTimeKind.Local).AddTicks(8150), "libyanlacc@gmail.com", "- Facebook", "- Instagram", null, "+218913832221", "- Twitter" });

            migrationBuilder.InsertData(
                table: "SiteInfo",
                columns: new[] { "Id", "About", "Activity", "CoverImageUrl", "Created", "LogoUrl", "Modified", "Name" },
                values: new object[] { new Guid("c60e3012-3eb4-49f0-9b67-547757beca32"), "", "LACC site", null, new DateTime(2025, 5, 12, 22, 48, 35, 146, DateTimeKind.Local).AddTicks(8007), null, null, "LACC" });

            migrationBuilder.InsertData(
                table: "SiteState",
                columns: new[] { "Id", "ClosingMessage", "Created", "Modified", "State" },
                values: new object[] { new Guid("5e56f722-70b2-4e1e-a858-6a2907370fb4"), "The site is temporarily closed for development", new DateTime(2025, 5, 12, 22, 48, 35, 146, DateTimeKind.Local).AddTicks(8173), null, true });

            migrationBuilder.AddForeignKey(
                name: "FK_AirPortRequestFiles_Landing_AirPortRequestId",
                table: "AirPortRequestFiles",
                column: "AirPortRequestId",
                principalTable: "Landing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Landing_AspNetUsers_ApproverUserId",
                table: "Landing",
                column: "ApproverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
