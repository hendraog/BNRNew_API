using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("ticket")]
    [Index("ticket_no", Name = "ticket_idx1", IsUnique = true)]
    public class Ticket
    {
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
        public Golongan golongan { get; set; }

        [Required]
        public string tuslah { get; set; }

        [Required]
        public string jenis_muatan { get; set; }

        [Required]
        public int? berat { get; set; }


        [Required]
        public int? volume { get; set; }

        [Required]
        public string keterangan { get; set; }

        [Required]
        public string plat_no { get; set; }

        [Required]
        public string nama { get; set; }

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


        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt{ get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
