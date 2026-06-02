using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserJob = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPic = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UpdateP = table.Column<bool>(type: "bit", nullable: false),
                    InsertP = table.Column<bool>(type: "bit", nullable: false),
                    PrintP = table.Column<bool>(type: "bit", nullable: false),
                    UsersP = table.Column<bool>(type: "bit", nullable: false),
                    SettingsP = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldUserId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblBranch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBranch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblCourt1",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourt1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowId = table.Column<int>(type: "int", nullable: false),
                    OldData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblExpenseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblExpenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblIncidentPlace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentPlaceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Del = table.Column<bool>(type: "bit", nullable: false),
                    QRCPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CourtsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblIncidentPlace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblPlainitiffName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlainitiffName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<long>(type: "bigint", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Del = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPlainitiffName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRoleClaims",
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
                    table.PrimaryKey("PK_AppRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRoleClaims_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserClaims",
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
                    table.PrimaryKey("PK_AppUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserClaims_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AppUserLogins_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AppUserTokens_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblCourtsN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlaintiffName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileIcon = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Setteld = table.Column<double>(type: "float", nullable: false),
                    Reserve = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourtsN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCourtsN_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblCourtsN_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClaimNumber = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ExpenseTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblExpenses_TblExpenseTypes_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "TblExpenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblFriendly",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlaintiffNameId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Setteld = table.Column<double>(type: "float", nullable: false),
                    Reserve = table.Column<double>(type: "float", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileIcon = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFriendly", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblFriendly_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblFriendly_TblPlainitiffName_PlaintiffNameId",
                        column: x => x.PlaintiffNameId,
                        principalTable: "TblPlainitiffName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblOutters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlaintiffNameId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    Setteld = table.Column<double>(type: "float", nullable: false),
                    Reserve = table.Column<double>(type: "float", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileIcon = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblOutters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblOutters_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblOutters_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblOutters_TblPlainitiffName_PlaintiffNameId",
                        column: x => x.PlaintiffNameId,
                        principalTable: "TblPlainitiffName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblCourtFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CourtsNId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourtFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCourtFiles_TblCourtsN_CourtsNId",
                        column: x => x.CourtsNId,
                        principalTable: "TblCourtsN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblFriendlyFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FriendlyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblFriendlyFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblFriendlyFiles_TblFriendly_FriendlyId",
                        column: x => x.FriendlyId,
                        principalTable: "TblFriendly",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblOutterFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    OutterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblOutterFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblOutterFiles_TblOutters_OutterId",
                        column: x => x.OutterId,
                        principalTable: "TblOutters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleClaims_RoleId",
                table: "AppRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AppRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserClaims_UserId",
                table: "AppUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserLogins_UserId",
                table: "AppUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoles_RoleId",
                table: "AppUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AppUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OldUserId",
                table: "AppUsers",
                column: "OldUserId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AppUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtFiles_CourtsNId",
                table: "TblCourtFiles",
                column: "CourtsNId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtCases_Year_Num",
                table: "TblCourtsN",
                columns: new[] { "Year", "Num" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtsN_BranchId",
                table: "TblCourtsN",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtsN_CourtId",
                table: "TblCourtsN",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblExpenses_ExpenseTypeId",
                table: "TblExpenses",
                column: "ExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TblFriendly_BranchId",
                table: "TblFriendly",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblFriendly_PlaintiffNameId",
                table: "TblFriendly",
                column: "PlaintiffNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TblFriendlyFiles_FriendlyId",
                table: "TblFriendlyFiles",
                column: "FriendlyId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOutterFiles_OutterId",
                table: "TblOutterFiles",
                column: "OutterId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOutters_BranchId",
                table: "TblOutters",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOutters_CourtId",
                table: "TblOutters",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOutters_PlaintiffNameId",
                table: "TblOutters",
                column: "PlaintiffNameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRoleClaims");

            migrationBuilder.DropTable(
                name: "AppUserClaims");

            migrationBuilder.DropTable(
                name: "AppUserLogins");

            migrationBuilder.DropTable(
                name: "AppUserRoles");

            migrationBuilder.DropTable(
                name: "AppUserTokens");

            migrationBuilder.DropTable(
                name: "TblCourtFiles");

            migrationBuilder.DropTable(
                name: "TblEvents");

            migrationBuilder.DropTable(
                name: "TblExpenses");

            migrationBuilder.DropTable(
                name: "TblFriendlyFiles");

            migrationBuilder.DropTable(
                name: "TblIncidentPlace");

            migrationBuilder.DropTable(
                name: "TblMessages");

            migrationBuilder.DropTable(
                name: "TblOutterFiles");

            migrationBuilder.DropTable(
                name: "AppRoles");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "TblCourtsN");

            migrationBuilder.DropTable(
                name: "TblExpenseTypes");

            migrationBuilder.DropTable(
                name: "TblFriendly");

            migrationBuilder.DropTable(
                name: "TblOutters");

            migrationBuilder.DropTable(
                name: "TblBranch");

            migrationBuilder.DropTable(
                name: "TblCourt1");

            migrationBuilder.DropTable(
                name: "TblPlainitiffName");
        }
    }
}
