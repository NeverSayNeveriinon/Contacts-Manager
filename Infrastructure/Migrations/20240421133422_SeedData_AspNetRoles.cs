using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class SeedData_AspNetRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("53253ccd-15c7-4c49-8021-a0f1633899c0"), "5050e615-22ca-404a-a319-24b672a7a333", "User", "USER" },
                    { new Guid("87ef14a9-2d76-43b4-9ca2-61f508d9b016"), "2ef91154-c51a-46aa-9eb7-84dd64cc6a74", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("53253ccd-15c7-4c49-8021-a0f1633899c0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("87ef14a9-2d76-43b4-9ca2-61f508d9b016"));
        }
    }
}
