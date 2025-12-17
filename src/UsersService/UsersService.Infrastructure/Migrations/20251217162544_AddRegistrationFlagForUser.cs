using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationFlagForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFullRegistration",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("01eb48d5-6f39-436f-b55a-a948f240241d"),
                column: "IsFullRegistration",
                value: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("11eb48d5-6f39-436f-b55a-a948f240241d"),
                column: "IsFullRegistration",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFullRegistration",
                table: "AspNetUsers");
        }
    }
}
