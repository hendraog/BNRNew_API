using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class initialdb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "user",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "user",
                newName: "role");
        }
    }
}
