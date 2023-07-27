using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace my_api.Migrations
{
    /// <inheritdoc />
    public partial class tambah3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "initial3",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "initial3",
                table: "Users");
        }
    }
}
