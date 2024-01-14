﻿// <auto-generated />
using System;
using BNRNew_API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BNRNew_API.Migrations
{
    [DbContext(typeof(MyDBContext))]
    partial class MyDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("BNRNew_API.Entities.CargoDetail", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("cargoManifestid")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ticket")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("cargoManifestid");

                    b.ToTable("cargo_detail");
                });

            modelBuilder.Entity("BNRNew_API.Entities.CargoManifest", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("alamat")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("lokasi_asal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("lokasi_tujuan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("manifest_no")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("nama_kapal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("nama_nahkoda")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("tgl_manifest")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex(new[] { "manifest_no" }, "manifest_idx1")
                        .IsUnique();

                    b.ToTable("cargo_manifest");
                });

            modelBuilder.Entity("BNRNew_API.Entities.Golongan", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("golongan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("harga")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<double?>("max_length")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<double?>("min_length")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.HasKey("id");

                    b.HasIndex(new[] { "golongan" }, "golongan_idx1")
                        .IsUnique();

                    b.ToTable("golongan");
                });

            modelBuilder.Entity("BNRNew_API.Entities.GolonganPlat", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<long>("golonganid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("plat_no")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex(new[] { "plat_no" }, "golongan_plat_idx1")
                        .IsUnique();

                    b.ToTable("golongan_plat");
                });

            modelBuilder.Entity("BNRNew_API.Entities.Sequence", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("key")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("last_used")
                        .HasColumnType("TEXT");

                    b.Property<int>("last_value")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex(new[] { "key" }, "sequence_idx1")
                        .IsUnique();

                    b.ToTable("sequence");
                });

            modelBuilder.Entity("BNRNew_API.Entities.Ticket", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("alamat_supir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("asal_supir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("berat")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<long?>("biaya_tuslah")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("cargoDetail")
                        .HasColumnType("INTEGER");

                    b.Property<long>("golongan")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("harga")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<string>("jenis_muatan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("jumlah_orang")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<string>("keterangan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("lebar_kenderaan")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<string>("lokasi_asal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("lokasi_tujuan")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("nama_pengurus")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("nama_supir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("panjang_kenderaan")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<double?>("panjang_ori_kenderaan")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<string>("plat_no")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("printer_count")
                        .HasColumnType("INTEGER");

                    b.Property<string>("tally_no")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("tanggal_berlaku")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("tanggal_masuk")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ticket_no")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("tinggi_kenderaan")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.Property<long?>("total_harga")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<string>("tujuan_supir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("tuslah")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("volume")
                        .IsRequired()
                        .HasColumnType("REAL");

                    b.HasKey("id");

                    b.HasIndex(new[] { "ticket_no" }, "ticket_idx1")
                        .IsUnique();

                    b.HasIndex(new[] { "tally_no" }, "ticket_idx2")
                        .IsUnique();

                    b.ToTable("ticket");
                });

            modelBuilder.Entity("BNRNew_API.Entities.User", b =>
                {
                    b.Property<long?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Active")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex(new[] { "UserName" }, "user_idx1")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("BNRNew_API.Entities.CargoDetail", b =>
                {
                    b.HasOne("BNRNew_API.Entities.CargoManifest", "cargoManifest")
                        .WithMany("detailData")
                        .HasForeignKey("cargoManifestid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("cargoManifest");
                });

            modelBuilder.Entity("BNRNew_API.Entities.CargoManifest", b =>
                {
                    b.Navigation("detailData");
                });
#pragma warning restore 612, 618
        }
    }
}
