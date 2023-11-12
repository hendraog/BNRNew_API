using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "key1",
                table: "user",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "user_idx2",
                table: "user",
                columns: new[] { "Role", "key1" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "user_idx2",
                table: "user");

            migrationBuilder.DropColumn(
                name: "key1",
                table: "user");
        }
    }
}
