using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class migrate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tally_no",
                table: "ticket",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ticket_idx2",
                table: "ticket",
                column: "tally_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ticket_idx2",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "tally_no",
                table: "ticket");
        }
    }
}
