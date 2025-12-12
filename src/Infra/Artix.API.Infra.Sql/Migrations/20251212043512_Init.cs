using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Major = table.Column<int>(type: "int", nullable: false),
                    Minor = table.Column<int>(type: "int", nullable: false),
                    Patch = table.Column<int>(type: "int", nullable: false),
                    VersionString = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "CAST([Major] AS NVARCHAR(10)) + '.' + CAST([Minor] AS NVARCHAR(10)) + '.' + CAST([Patch] AS NVARCHAR(10))"),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    MinSupported = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPro = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedBy = table.Column<long>(type: "bigint", maxLength: 100, nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalPeriods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Museums",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Museums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    IsBroadcast = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    LastErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GeneralInformation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecialInformation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: true),
                    Tier = table.Column<int>(type: "int", nullable: true),
                    IsSpecial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ObjectSaleType = table.Column<int>(type: "int", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Error = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TierConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinScanCount = table.Column<int>(type: "int", nullable: false),
                    RequiredUpgraded = table.Column<bool>(type: "bit", nullable: true),
                    RequiredInCollection = table.Column<bool>(type: "bit", nullable: true),
                    MinDaysSinceAcquired = table.Column<int>(type: "int", nullable: true),
                    RequiredSpecial = table.Column<bool>(type: "bit", nullable: true),
                    RequiredSaleType = table.Column<int>(type: "int", nullable: true),
                    RequiredMembershipType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredActiveStreak = table.Column<bool>(type: "bit", nullable: true),
                    RequiredCoOpKey = table.Column<bool>(type: "bit", nullable: true),
                    TierLevel = table.Column<int>(type: "int", nullable: false),
                    Multiplier = table.Column<double>(type: "float", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
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
                    UserId = table.Column<long>(type: "bigint", nullable: false)
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
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
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
                    UserId = table.Column<long>(type: "bigint", nullable: false),
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
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FriendId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => new { x.UserId, x.FriendId });
                    table.CheckConstraint("CK_Friendships_NotSelf", "[UserId] <> [FriendId]");
                    table.ForeignKey(
                        name: "FK_Friendships_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStrikes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StrikeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StrikeCount = table.Column<int>(type: "int", nullable: false),
                    LastInteraction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FuelCount = table.Column<int>(type: "int", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStrikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStrikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserXps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TotalXp = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserXps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserXps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserImages",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => new { x.FileId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserImages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserImages_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MuseumImages",
                columns: table => new
                {
                    MuseumId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuseumImages", x => new { x.FileId, x.MuseumId });
                    table.ForeignKey(
                        name: "FK_MuseumImages_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MuseumImages_Museums_MuseumId",
                        column: x => x.MuseumId,
                        principalTable: "Museums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMuseumKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    MuseumId = table.Column<long>(type: "bigint", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "bit", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShareCount = table.Column<int>(type: "int", nullable: false),
                    IsShared = table.Column<bool>(type: "bit", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMuseumKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMuseumKeys_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMuseumKeys_Museums_MuseumId",
                        column: x => x.MuseumId,
                        principalTable: "Museums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotification_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SketchUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntries_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricePoints = table.Column<int>(type: "int", nullable: true),
                    ListedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSold = table.Column<bool>(type: "bit", nullable: true),
                    ObjectId = table.Column<long>(type: "bigint", nullable: true),
                    SellerId = table.Column<long>(type: "bigint", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceItems_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketplaceItems_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MuseumObjects",
                columns: table => new
                {
                    MuseumId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuseumObjects", x => new { x.MuseumId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_MuseumObjects_Museums_MuseumId",
                        column: x => x.MuseumId,
                        principalTable: "Museums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MuseumObjects_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectHistoricalPeriods",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    HistoricalPeriodId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectHistoricalPeriods", x => new { x.ObjectId, x.HistoricalPeriodId });
                    table.ForeignKey(
                        name: "FK_ObjectHistoricalPeriods_HistoricalPeriods_HistoricalPeriodId",
                        column: x => x.HistoricalPeriodId,
                        principalTable: "HistoricalPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectHistoricalPeriods_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectImages",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectImages", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectImages_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectImages_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectModels",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectModels", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectModels_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectModels_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserScans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    ScanCount = table.Column<int>(type: "int", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsUpgraded = table.Column<bool>(type: "bit", nullable: false),
                    InCollection = table.Column<bool>(type: "bit", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserScans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserScans_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XpReward = table.Column<int>(type: "int", nullable: false),
                    IsPro = table.Column<bool>(type: "bit", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonTasks_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSeasonProgresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    TotalXp = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSeasonProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSeasonProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSeasonProgresses_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceTracks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Artist = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsFree = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: true),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceTracks_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoiceTracks_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObjectTypes",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectTypes", x => new { x.TypeId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectTypes_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectTypes_Types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    CollectionId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => new { x.CollectionId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_CollectionItems_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserJournalEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnlockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JournalEntryId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserJournalEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJournalEntries_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceTrackFiles",
                columns: table => new
                {
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    VoiceTrackId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceTrackFiles", x => new { x.FileId, x.VoiceTrackId });
                    table.ForeignKey(
                        name: "FK_VoiceTrackFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoiceTrackFiles_VoiceTracks_FileId",
                        column: x => x.FileId,
                        principalTable: "VoiceTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVersion_BusinessId",
                table: "AppVersions",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVersion_BusinessId_IsDeleted",
                table: "AppVersions",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AppVersion_CreatedAt",
                table: "AppVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppVersion_IsDeleted",
                table: "AppVersions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_Name",
                table: "AspNetRoles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_NormalizedName",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                name: "IX_AppUsers_DisplayName",
                table: "AspNetUsers",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_IsPro",
                table: "AspNetUsers",
                column: "IsPro");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_CollectionId",
                table: "CollectionItems",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_ObjectId",
                table: "CollectionItems",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_BusinessId",
                table: "Collections",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_BusinessId_IsDeleted",
                table: "Collections",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Collection_CreatedAt",
                table: "Collections",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_IsDeleted",
                table: "Collections",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileEntity_BusinessId",
                table: "Files",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_FileEntity_BusinessId_IsDeleted",
                table: "Files",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_FileEntity_CreatedAt",
                table: "Files",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileEntity_IsDeleted",
                table: "Files",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendships",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId_FriendId",
                table: "Friendships",
                columns: new[] { "UserId", "FriendId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPeriod_BusinessId",
                table: "HistoricalPeriods",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPeriod_BusinessId_IsDeleted",
                table: "HistoricalPeriods",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPeriod_CreatedAt",
                table: "HistoricalPeriods",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPeriod_IsDeleted",
                table: "HistoricalPeriods",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPeriods_Name",
                table: "HistoricalPeriods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ObjectId",
                table: "JournalEntries",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_BusinessId",
                table: "JournalEntries",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_BusinessId_IsDeleted",
                table: "JournalEntries",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_CreatedAt",
                table: "JournalEntries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_IsDeleted",
                table: "JournalEntries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItem_BusinessId",
                table: "MarketplaceItems",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItem_BusinessId_IsDeleted",
                table: "MarketplaceItems",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItem_CreatedAt",
                table: "MarketplaceItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItem_IsDeleted",
                table: "MarketplaceItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_ObjectId",
                table: "MarketplaceItems",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_SellerId",
                table: "MarketplaceItems",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_FileId",
                table: "MuseumImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_ObjectId",
                table: "MuseumImages",
                column: "MuseumId");

            migrationBuilder.CreateIndex(
                name: "IX_MuseumObject_MuseumId",
                table: "MuseumObjects",
                column: "MuseumId");

            migrationBuilder.CreateIndex(
                name: "IX_MuseumObject_ObjectId",
                table: "MuseumObjects",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Museum_BusinessId",
                table: "Museums",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Museum_BusinessId_IsDeleted",
                table: "Museums",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Museum_CreatedAt",
                table: "Museums",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Museum_IsDeleted",
                table: "Museums",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Museums_IsActive",
                table: "Museums",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Museums_Name",
                table: "Museums",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BusinessId",
                table: "Notifications",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_BusinessId_IsDeleted",
                table: "Notifications",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IsDeleted",
                table: "Notifications",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHistoricalPeriods_HistoricalPeriodId",
                table: "ObjectHistoricalPeriods",
                column: "HistoricalPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHistoricalPeriods_ObjectId",
                table: "ObjectHistoricalPeriods",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectImageFiles_FileId",
                table: "ObjectImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectImageFiles_ObjectId",
                table: "ObjectImages",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectModelFiles_FileId",
                table: "ObjectModels",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectModelFiles_ObjectId",
                table: "ObjectModels",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Object_BusinessId",
                table: "Objects",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Object_BusinessId_IsDeleted",
                table: "Objects",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Object_CreatedAt",
                table: "Objects",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Object_IsDeleted",
                table: "Objects",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_Name",
                table: "Objects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_QrCode",
                table: "Objects",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTypes_ObjectId",
                table: "ObjectTypes",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTypes_TypeId",
                table: "ObjectTypes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_BusinessId",
                table: "OTPs",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_BusinessId_IsDeleted",
                table: "OTPs",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_OTP_Code",
                table: "OTPs",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_CreatedAt",
                table: "OTPs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_ExpiresAt",
                table: "OTPs",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_IsDeleted",
                table: "OTPs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_PhoneNumber",
                table: "OTPs",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_PhoneNumber_Code",
                table: "OTPs",
                columns: new[] { "PhoneNumber", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_OTP_PhoneNumber_ExpiresAt",
                table: "OTPs",
                columns: new[] { "PhoneNumber", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAt",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "Status", "CreatedAt" },
                filter: "\"Status\" = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_Season_BusinessId",
                table: "Seasons",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Season_BusinessId_IsDeleted",
                table: "Seasons",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Season_CreatedAt",
                table: "Seasons",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Season_IsDeleted",
                table: "Seasons",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTask_BusinessId",
                table: "SeasonTasks",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTask_BusinessId_IsDeleted",
                table: "SeasonTasks",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTask_CreatedAt",
                table: "SeasonTasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTask_IsDeleted",
                table: "SeasonTasks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTasks_SeasonId",
                table: "SeasonTasks",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_TierConfig_BusinessId",
                table: "TierConfigs",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_TierConfig_BusinessId_IsDeleted",
                table: "TierConfigs",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TierConfig_CreatedAt",
                table: "TierConfigs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TierConfig_IsDeleted",
                table: "TierConfigs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Category_BusinessId",
                table: "Types",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_BusinessId_IsDeleted",
                table: "Types",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedAt",
                table: "Types",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted",
                table: "Types",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Types_Name",
                table: "Types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserImageFiles_FileId",
                table: "UserImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserImageFiles_UserId",
                table: "UserImages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntries_JournalEntryId",
                table: "UserJournalEntries",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntries_UserId",
                table: "UserJournalEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntry_BusinessId",
                table: "UserJournalEntries",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntry_BusinessId_IsDeleted",
                table: "UserJournalEntries",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntry_CreatedAt",
                table: "UserJournalEntries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserJournalEntry_IsDeleted",
                table: "UserJournalEntries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistories_UserId",
                table: "UserLoginHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_BusinessId",
                table: "UserLoginHistories",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_BusinessId_IsDeleted",
                table: "UserLoginHistories",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_CreatedAt",
                table: "UserLoginHistories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_IsDeleted",
                table: "UserLoginHistories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKey_BusinessId",
                table: "UserMuseumKeys",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKey_BusinessId_IsDeleted",
                table: "UserMuseumKeys",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKey_CreatedAt",
                table: "UserMuseumKeys",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKey_IsDeleted",
                table: "UserMuseumKeys",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKeys_MuseumId",
                table: "UserMuseumKeys",
                column: "MuseumId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMuseumKeys_UserId",
                table: "UserMuseumKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_BusinessId",
                table: "UserNotification",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_BusinessId_IsDeleted",
                table: "UserNotification",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_CreatedAt",
                table: "UserNotification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_IsDeleted",
                table: "UserNotification",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_NotificationId",
                table: "UserNotification",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_UserId",
                table: "UserNotification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserScan_BusinessId",
                table: "UserScans",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserScan_BusinessId_IsDeleted",
                table: "UserScans",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScan_CreatedAt",
                table: "UserScans",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserScan_IsDeleted",
                table: "UserScans",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserScans_ObjectId",
                table: "UserScans",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserScans_UserId",
                table: "UserScans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgress_BusinessId",
                table: "UserSeasonProgresses",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgress_BusinessId_IsDeleted",
                table: "UserSeasonProgresses",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgress_CreatedAt",
                table: "UserSeasonProgresses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgress_IsDeleted",
                table: "UserSeasonProgresses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgresses_SeasonId",
                table: "UserSeasonProgresses",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeasonProgresses_UserId",
                table: "UserSeasonProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStrike_BusinessId",
                table: "UserStrikes",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStrike_BusinessId_IsDeleted",
                table: "UserStrikes",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserStrike_CreatedAt",
                table: "UserStrikes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserStrike_IsDeleted",
                table: "UserStrikes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserStrikes_UserId",
                table: "UserStrikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserXp_BusinessId",
                table: "UserXps",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserXp_BusinessId_IsDeleted",
                table: "UserXps",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserXp_CreatedAt",
                table: "UserXps",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserXp_IsDeleted",
                table: "UserXps",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserXps_UserId",
                table: "UserXps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrackFiles_FileId",
                table: "VoiceTrackFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrackFiles_VoiceTrackId",
                table: "VoiceTrackFiles",
                column: "VoiceTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicTracks_Artist",
                table: "VoiceTracks",
                column: "Artist");

            migrationBuilder.CreateIndex(
                name: "IX_MusicTracks_SeasonId",
                table: "VoiceTracks",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicTracks_Title",
                table: "VoiceTracks",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrack_BusinessId",
                table: "VoiceTracks",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrack_BusinessId_IsDeleted",
                table: "VoiceTracks",
                columns: new[] { "BusinessId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrack_CreatedAt",
                table: "VoiceTracks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTrack_IsDeleted",
                table: "VoiceTracks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceTracks_ObjectId",
                table: "VoiceTracks",
                column: "ObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersions");

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
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "MarketplaceItems");

            migrationBuilder.DropTable(
                name: "MuseumImages");

            migrationBuilder.DropTable(
                name: "MuseumObjects");

            migrationBuilder.DropTable(
                name: "ObjectHistoricalPeriods");

            migrationBuilder.DropTable(
                name: "ObjectImages");

            migrationBuilder.DropTable(
                name: "ObjectModels");

            migrationBuilder.DropTable(
                name: "ObjectTypes");

            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "SeasonTasks");

            migrationBuilder.DropTable(
                name: "TierConfigs");

            migrationBuilder.DropTable(
                name: "UserImages");

            migrationBuilder.DropTable(
                name: "UserJournalEntries");

            migrationBuilder.DropTable(
                name: "UserLoginHistories");

            migrationBuilder.DropTable(
                name: "UserMuseumKeys");

            migrationBuilder.DropTable(
                name: "UserNotification");

            migrationBuilder.DropTable(
                name: "UserScans");

            migrationBuilder.DropTable(
                name: "UserSeasonProgresses");

            migrationBuilder.DropTable(
                name: "UserStrikes");

            migrationBuilder.DropTable(
                name: "UserXps");

            migrationBuilder.DropTable(
                name: "VoiceTrackFiles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "HistoricalPeriods");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "Museums");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "VoiceTracks");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
