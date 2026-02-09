using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFcmTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "FcmNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClickAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Target = table.Column<int>(type: "int", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetDeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SuccessCount = table.Column<int>(type: "int", nullable: false),
                    FailureCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcmNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FcmNotifications_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FcmTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcmTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FcmTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FcmNotificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcmNotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FcmNotificationLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FcmNotificationLogs_FcmNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "FcmNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FcmNotificationLogs_FcmTokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "FcmTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FcmNotificationLogs_NotificationId",
                table: "FcmNotificationLogs",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FcmNotificationLogs_TokenId",
                table: "FcmNotificationLogs",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_FcmNotificationLogs_UserId",
                table: "FcmNotificationLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FcmNotifications_TargetUserId",
                table: "FcmNotifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FcmTokens_UserId",
                table: "FcmTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FcmNotificationLogs");

            migrationBuilder.DropTable(
                name: "FcmNotifications");

            migrationBuilder.DropTable(
                name: "FcmTokens");
        }
    }
}
