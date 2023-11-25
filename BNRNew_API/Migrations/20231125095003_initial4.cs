using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "golongan",
                table: "golongan_plat");

            migrationBuilder.AddColumn<long>(
                name: "golonganid",
                table: "golongan_plat",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_golongan_plat_golonganid",
                table: "golongan_plat",
                column: "golonganid");

            migrationBuilder.AddForeignKey(
                name: "FK_golongan_plat_golongan_golonganid",
                table: "golongan_plat",
                column: "golonganid",
                principalTable: "golongan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_golongan_plat_golongan_golonganid",
                table: "golongan_plat");

            migrationBuilder.DropIndex(
                name: "IX_golongan_plat_golonganid",
                table: "golongan_plat");

            migrationBuilder.DropColumn(
                name: "golonganid",
                table: "golongan_plat");

            migrationBuilder.AddColumn<string>(
                name: "golongan",
                table: "golongan_plat",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
