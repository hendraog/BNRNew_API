using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace my_api.Migrations
{
    /// <inheritdoc />
    public partial class TambahInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "initial",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "initial",
                table: "Users");
        }
    }
}
