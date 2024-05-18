using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "Countries",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "Persons",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: false),
                    TINumber = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true, defaultValue: "ABCDEFGH123"),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.ID);
                    table.CheckConstraint("CHK_TINumber", "len([TINumber]) = 11");
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryID",
                        column: x => x.CountryID,
                        principalTable: "Countries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("211d03ae-07fe-4cb4-813e-163f46568b44"), "4A191780-214E-47D6-83B8-4345A61E804C", "User", "USER" },
                    { new Guid("a9c2bc35-61fe-4e60-8158-2bfd6e1478eb"), "7DB6149D-F1EE-4A0B-BA7B-D99E41414D73", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), "USA" },
                    { new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"), "Germany" },
                    { new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"), "Sweden" },
                    { new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"), "England" },
                    { new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), "Italy" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "ID", "Address", "CountryID", "DateOfBirth", "Email", "Gender", "Name", "ReceiveNewsLetters" },
                values: new object[,]
                {
                    { new Guid("272d82b2-dfd8-4005-aae5-84ec368a3d28"), "NewYork, st 88", new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), new DateTime(1992, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "chandler.fake.mail@gmail.com", "Male", "Chandler", false },
                    { new Guid("2917892d-5864-4356-aeea-d1ce7c11155d"), "NewYork, st 89", new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), new DateTime(1995, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "rachel.fake.mail@gmail.com", "Female", "Rachel", true },
                    { new Guid("29339209-63f5-492f-8459-754943c74abf"), "57449 Brown Way", new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), new DateTime(1983, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "mjarrell6@wisc.edu", "Male", "Maddy", true },
                    { new Guid("2a6d3738-9def-43ac-9279-0310edc7ceca"), "97570 Raven Circle", new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "mlingfoot5@netvibes.com", "Male", "Mitchael", false },
                    { new Guid("3df46ad3-b1ef-4b7e-9a9b-174e25d3571c"), "London, st 49", new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"), new DateTime(1996, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "nora.fake.mail@gmail.com", "Female", "Nora", true },
                    { new Guid("59163933-774f-4ecb-bdbc-bd459ea56499"), "London, st 28", new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"), new DateTime(1998, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "kevin.fake.mail@gmail.com", "Male", "Kevin", true },
                    { new Guid("59e8021f-5d37-4a98-83a7-01ea39914b02"), "Stockholm, st 58", new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"), new DateTime(2004, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "jill.fake.mail@gmail.com", "Female", "kevinJill", false },
                    { new Guid("a3b9833b-8a4d-43e9-8690-61e08df81a9a"), "9334 Fremont Street", new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"), new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "vklussb@nationalgeographic.com", "Female", "Verene", true },
                    { new Guid("c03bbe45-9aeb-4d24-99e0-4743016ffce9"), "4 Parkside Point", new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"), new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "mwebsdale0@people.com.cn", "Female", "Marguerite", false },
                    { new Guid("c6d50a47-f7e6-4482-8be0-4ddfc057fa6e"), "73 Heath Avenue", new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"), new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "fbowsher2@howstuffworks.com", "Male", "Franchot", true },
                    { new Guid("cb035f22-e7cf-4907-bd07-91cfee5240f3"), "484 Clarendon Court", new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), new DateTime(1997, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "lwoodwing9@wix.com", "Male", "Lombard", false }
                });

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
                name: "IX_Persons_CountryID",
                table: "Persons",
                column: "CountryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Persons");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
