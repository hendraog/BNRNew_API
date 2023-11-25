using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "min_length",
                table: "golongan",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<double>(
                name: "max_length",
                table: "golongan",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<double>(
                name: "harga",
                table: "golongan",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "cargo_manifest",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    manifest_no = table.Column<string>(type: "TEXT", nullable: false),
                    tgl_manifest = table.Column<DateTime>(type: "TEXT", nullable: false),
                    nama_kapal = table.Column<string>(type: "TEXT", nullable: false),
                    nama_nahkoda = table.Column<string>(type: "TEXT", nullable: false),
                    lokasi_asal = table.Column<string>(type: "TEXT", nullable: false),
                    lokasi_tujuan = table.Column<string>(type: "TEXT", nullable: false),
                    alamat = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargo_manifest", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "golongan_plat",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    plat_no = table.Column<string>(type: "TEXT", nullable: false),
                    golongan = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_golongan_plat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cargo_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cargoManifestid = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargo_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_cargo_detail_cargo_manifest_cargoManifestid",
                        column: x => x.cargoManifestid,
                        principalTable: "cargo_manifest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ticket_no = table.Column<string>(type: "TEXT", nullable: false),
                    tanggal_masuk = table.Column<DateTime>(type: "TEXT", nullable: false),
                    tanggal_berlaku = table.Column<DateTime>(type: "TEXT", nullable: false),
                    lokasi_asal = table.Column<int>(type: "INTEGER", nullable: false),
                    lokasi_tujuan = table.Column<int>(type: "INTEGER", nullable: false),
                    golonganid = table.Column<long>(type: "INTEGER", nullable: false),
                    tuslah = table.Column<string>(type: "TEXT", nullable: false),
                    jenis_muatan = table.Column<string>(type: "TEXT", nullable: false),
                    berat = table.Column<double>(type: "REAL", nullable: false),
                    volume = table.Column<double>(type: "REAL", nullable: false),
                    keterangan = table.Column<string>(type: "TEXT", nullable: false),
                    plat_no = table.Column<string>(type: "TEXT", nullable: false),
                    nama = table.Column<string>(type: "TEXT", nullable: false),
                    jumlah_orang = table.Column<int>(type: "INTEGER", nullable: false),
                    harga = table.Column<long>(type: "INTEGER", nullable: false),
                    biaya_tuslah = table.Column<long>(type: "INTEGER", nullable: false),
                    total_harga = table.Column<long>(type: "INTEGER", nullable: false),
                    panjang_ori_kenderaan = table.Column<double>(type: "REAL", nullable: false),
                    tinggi_kenderaan = table.Column<double>(type: "REAL", nullable: false),
                    lebar_kenderaan = table.Column<double>(type: "REAL", nullable: false),
                    panjang_kenderaan = table.Column<double>(type: "REAL", nullable: false),
                    nama_pengurus = table.Column<string>(type: "TEXT", nullable: false),
                    alamat_supir = table.Column<string>(type: "TEXT", nullable: false),
                    asal_supir = table.Column<string>(type: "TEXT", nullable: false),
                    tujuan_supir = table.Column<string>(type: "TEXT", nullable: false),
                    CargoDetail = table.Column<long>(type: "INTEGER", nullable: true),
                    printer_count = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket", x => x.id);
                    table.ForeignKey(
                        name: "FK_ticket_cargo_detail_CargoDetail",
                        column: x => x.CargoDetail,
                        principalTable: "cargo_detail",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ticket_golongan_golonganid",
                        column: x => x.golonganid,
                        principalTable: "golongan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cargo_detail_cargoManifestid",
                table: "cargo_detail",
                column: "cargoManifestid");

            migrationBuilder.CreateIndex(
                name: "manifest_idx1",
                table: "cargo_manifest",
                column: "manifest_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "golongan_plat_idx1",
                table: "golongan_plat",
                column: "plat_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_CargoDetail",
                table: "ticket",
                column: "CargoDetail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_golonganid",
                table: "ticket",
                column: "golonganid");

            migrationBuilder.CreateIndex(
                name: "ticket_idx1",
                table: "ticket",
                column: "ticket_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "golongan_plat");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "cargo_detail");

            migrationBuilder.DropTable(
                name: "cargo_manifest");

            migrationBuilder.AlterColumn<int>(
                name: "min_length",
                table: "golongan",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "max_length",
                table: "golongan",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<long>(
                name: "harga",
                table: "golongan",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}
