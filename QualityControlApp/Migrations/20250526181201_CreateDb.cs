using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityControlApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    LastAccessTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Approval = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChronicDisease",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicDisease", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Twitter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instagram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCategoryType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCategoryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    State = table.Column<bool>(type: "bit", nullable: false),
                    ClosingMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirPortRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AircraftType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftRegistration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightPurpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryExitPoints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PilotName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExitPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedEntryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedExitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CargoDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrewCount = table.Column<int>(type: "int", nullable: true),
                    CrewNationalities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPortRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirPortRequests_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaritalStatus = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Landing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfFlights = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftRegistration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RadioCallSign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirportOfLanding = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ETA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ETD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurposeOfFlight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NatureOfPaxOrCargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptainNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptainNationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CrewDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AocDocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Landing_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AocNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_CompanyType_CompanyTypeId",
                        column: x => x.CompanyTypeId,
                        principalTable: "CompanyType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "QuestionType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionCategoryTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionType_QuestionCategoryType_QuestionCategoryTypeId",
                        column: x => x.QuestionCategoryTypeId,
                        principalTable: "QuestionCategoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChronicDiseaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiagnosisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRecord_ChronicDisease_ChronicDiseaseId",
                        column: x => x.ChronicDiseaseId,
                        principalTable: "ChronicDisease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HealthRecord_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AirPortRequestFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirPortRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LandingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Inspect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nots = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPortRequestFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirPortRequestFiles_AirPortRequests_AirPortRequestId",
                        column: x => x.AirPortRequestId,
                        principalTable: "AirPortRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirPortRequestFiles_FileType_FileTypeId",
                        column: x => x.FileTypeId,
                        principalTable: "FileType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirPortRequestFiles_Landing_LandingId",
                        column: x => x.LandingId,
                        principalTable: "Landing",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingAppointment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingAppointment_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    SaftyGrid = table.Column<int>(type: "int", nullable: false),
                    SqurtyGrid = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Num = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyQuestion_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyQuestion_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyQuestion_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxGrid = table.Column<int>(type: "int", nullable: false),
                    SectionContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyQuestionAssignedUsers",
                columns: table => new
                {
                    AssignedCompanyQuestionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyQuestionAssignedUsers", x => new { x.AssignedCompanyQuestionsId, x.AssignedUsersId });
                    table.ForeignKey(
                        name: "FK_CompanyQuestionAssignedUsers_AspNetUsers_AssignedUsersId",
                        column: x => x.AssignedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyQuestionAssignedUsers_CompanyQuestion_AssignedCompanyQuestionsId",
                        column: x => x.AssignedCompanyQuestionsId,
                        principalTable: "CompanyQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyQuestionContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CompanyQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: true),
                    Inspect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nots = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyQuestionContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyQuestionContent_CompanyQuestion_CompanyQuestionId",
                        column: x => x.CompanyQuestionId,
                        principalTable: "CompanyQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyQuestionContent_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirPortRequestFiles_AirPortRequestId",
                table: "AirPortRequestFiles",
                column: "AirPortRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AirPortRequestFiles_FileTypeId",
                table: "AirPortRequestFiles",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AirPortRequestFiles_LandingId",
                table: "AirPortRequestFiles",
                column: "LandingId");

            migrationBuilder.CreateIndex(
                name: "IX_AirPortRequests_ApproverUserId",
                table: "AirPortRequests",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAppointment_CompanyId",
                table: "BookingAppointment",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CompanyTypeId",
                table: "Company",
                column: "CompanyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestion_CompanyId",
                table: "CompanyQuestion",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestion_CreatorId",
                table: "CompanyQuestion",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestion_LocationId",
                table: "CompanyQuestion",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestionAssignedUsers_AssignedUsersId",
                table: "CompanyQuestionAssignedUsers",
                column: "AssignedUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestionContent_CompanyQuestionId",
                table: "CompanyQuestionContent",
                column: "CompanyQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyQuestionContent_QuestionId",
                table: "CompanyQuestionContent",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTypeCategoryAvailable_CompanyTypeId",
                table: "CompanyTypeCategoryAvailable",
                column: "CompanyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTypeCategoryAvailable_QuestionCategoryTypeId",
                table: "CompanyTypeCategoryAvailable",
                column: "QuestionCategoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecord_ChronicDiseaseId",
                table: "HealthRecord",
                column: "ChronicDiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecord_EmployeeId",
                table: "HealthRecord",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Landing_ApproverUserId",
                table: "Landing",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypeId",
                table: "Question",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_QuestionCategoryTypeId",
                table: "QuestionType",
                column: "QuestionCategoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirPortRequestFiles");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingAppointment");

            migrationBuilder.DropTable(
                name: "CompanyQuestionAssignedUsers");

            migrationBuilder.DropTable(
                name: "CompanyQuestionContent");

            migrationBuilder.DropTable(
                name: "CompanyTypeCategoryAvailable");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "HealthRecord");

            migrationBuilder.DropTable(
                name: "SiteInfo");

            migrationBuilder.DropTable(
                name: "SiteState");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "AirPortRequests");

            migrationBuilder.DropTable(
                name: "FileType");

            migrationBuilder.DropTable(
                name: "Landing");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CompanyQuestion");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "ChronicDisease");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "QuestionType");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CompanyType");

            migrationBuilder.DropTable(
                name: "QuestionCategoryType");
        }
    }
}
