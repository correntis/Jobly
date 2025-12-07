using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UsersService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"), null, null, "User", "USER" },
                    { new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"), null, null, "Company", "COMPANY" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("01eb48d5-6f39-436f-b55a-a948f240241d"), 0, "4cac2a77-ba92-4925-8dc5-6ee76aefe20f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user@example.com", false, "Maksim", "Rusetski", false, null, "USER@EXAMPLE.COM", null, "AQAAAAIAAYagAAAAEO2U87owvdt6cLGcTjrd0pM7Z5hm/5DFR1A8uPZjQh9wb7XCYWcKDxMfRSEmJ0ec6Q==", null, false, "42JNKQDOFZBXPARN2I7WHT4MU236MCTK", false, null },
                    { new Guid("11eb48d5-6f39-436f-b55a-a948f240241d"), 0, "5cac2a77-ba92-4925-8dc5-6ee76aefe20f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "company@example.com", false, "Maksim", "Rusetski", false, null, "COMPANY@EXAMPLE.COM", null, "AQAAAAIAAYagAAAAEO2U87owvdt6cLGcTjrd0pM7Z5hm/5DFR1A8uPZjQh9wb7XCYWcKDxMfRSEmJ0ec6Q==", null, false, "52JNKQDOFZBXPARN2I7WHT4MU236MCTK", false, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"), new Guid("01eb48d5-6f39-436f-b55a-a948f240241d") },
                    { new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"), new Guid("11eb48d5-6f39-436f-b55a-a948f240241d") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"), new Guid("01eb48d5-6f39-436f-b55a-a948f240241d") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"), new Guid("11eb48d5-6f39-436f-b55a-a948f240241d") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("01eb48d5-6f39-436f-b55a-a948f240241d"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("11eb48d5-6f39-436f-b55a-a948f240241d"));
        }
    }
}
