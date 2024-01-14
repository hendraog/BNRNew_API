using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNRNew_API.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "golongan",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    golongan = table.Column<string>(type: "TEXT", nullable: false),
                    harga = table.Column<double>(type: "REAL", nullable: false),
                    min_length = table.Column<double>(type: "REAL", nullable: false),
                    max_length = table.Column<double>(type: "REAL", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_golongan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "golongan_plat",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    plat_no = table.Column<string>(type: "TEXT", nullable: false),
                    golonganid = table.Column<long>(type: "INTEGER", nullable: false),
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
                name: "sequence",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    key = table.Column<int>(type: "INTEGER", nullable: false),
                    last_value = table.Column<int>(type: "INTEGER", nullable: false),
                    last_used = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sequence", x => x.id);
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
                    lokasi_asal = table.Column<string>(type: "TEXT", nullable: false),
                    lokasi_tujuan = table.Column<string>(type: "TEXT", nullable: false),
                    golongan = table.Column<long>(type: "INTEGER", nullable: false),
                    tuslah = table.Column<string>(type: "TEXT", nullable: false),
                    jenis_muatan = table.Column<string>(type: "TEXT", nullable: false),
                    berat = table.Column<double>(type: "REAL", nullable: false),
                    volume = table.Column<double>(type: "REAL", nullable: false),
                    keterangan = table.Column<string>(type: "TEXT", nullable: false),
                    plat_no = table.Column<string>(type: "TEXT", nullable: false),
                    nama_supir = table.Column<string>(type: "TEXT", nullable: false),
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
                    cargoDetail = table.Column<long>(type: "INTEGER", nullable: true),
                    printer_count = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cargo_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cargoManifestid = table.Column<long>(type: "INTEGER", nullable: false),
                    ticket = table.Column<long>(type: "INTEGER", nullable: false),
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
                name: "golongan_idx1",
                table: "golongan",
                column: "golongan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "golongan_plat_idx1",
                table: "golongan_plat",
                column: "plat_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "sequence_idx1",
                table: "sequence",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ticket_idx1",
                table: "ticket",
                column: "ticket_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_idx1",
                table: "user",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cargo_detail");

            migrationBuilder.DropTable(
                name: "golongan");

            migrationBuilder.DropTable(
                name: "golongan_plat");

            migrationBuilder.DropTable(
                name: "sequence");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "cargo_manifest");
        }
    }
}
