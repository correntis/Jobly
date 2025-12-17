using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnpForCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unp",
                table: "Companies",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unp",
                table: "Companies");
        }
    }
}
