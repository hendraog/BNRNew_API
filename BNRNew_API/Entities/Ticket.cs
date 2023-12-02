using BNRNew_API.config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static BNRNew_API.config.AppConstant;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("ticket")]
    [Index("ticket_no", Name = "ticket_idx1", IsUnique = true)]
    public class Ticket
    {

        public Ticket()
        {
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string ticket_no { get; set; }

        [Required]
        public DateTime? tanggal_masuk { get; set; }

        [Required]
        public DateTime? tanggal_berlaku { get; set; }

        [Required]
        public string lokasi_asal { get; set; }

        [Required]
        public string lokasi_tujuan { get; set; }

        [Required]
        public long golongan { get; set; }

        [Required]
        public string tuslah { get; set; }

        [Required]
        public string jenis_muatan { get; set; }

        [Required]
        public double? berat { get; set; }

        [Required]
        public double? volume { get; set; }

        [Required]
        public string keterangan { get; set; }

        [Required]
        public string plat_no { get; set; }

        [Required]
        public string nama_supir { get; set; }

        [Required]
        public int? jumlah_orang { get; set; }

        [Required]
        public long? harga { get; set; }

        [Required]
        public long? biaya_tuslah { get; set; }

        [Required]
        public long? total_harga { get; set; }

        [Required]
        public double? panjang_ori_kenderaan { get; set; }

        [Required]
        public double? tinggi_kenderaan { get; set; }

        [Required]
        public double? lebar_kenderaan { get; set; }

        [Required]
        public double? panjang_kenderaan { get; set; }

        //Data supir
        [Required]
        public string nama_pengurus { get; set; }

        [Required]
        public string alamat_supir { get; set; }

        [Required]
        public string asal_supir { get; set; }

        [Required]
        public string tujuan_supir { get; set; }

        public long? cargoDetail { get; set; }

        [Required]
        public int? printer_count { get; set; } = 0;


        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }


        [NotMapped]
        public string CreatedByName { get; set; }

        [NotMapped]
        public string UpdatedByName { get; set; }




        [NotMapped]
        public string golongan_name { get; set; }

    }
}
