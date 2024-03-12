using BNRNew_API.config;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static BNRNew_API.config.AppConstant;

namespace BNRNew_API.ticket.request
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class UpdateTicketRequest
    {
        public long? id { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long golongan { get; set; }

        [Required]
        public string tuslah { get; set; }

        [Required]
        public string jenis_muatan { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        public double? berat { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        public double? volume { get; set; }

        public string keterangan { get; set; }

        private string _plat_no;

        [Required]
        public string plat_no
        {
            get
            {
                return _plat_no.ToUpper();
            }
            set { _plat_no = value?.ToUpper(); }
        }


        [Required]
        public string nama_supir { get; set; }

        [Required]
        [Range(0, 1000)]
        public int? jumlah_orang { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long? harga { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long? biaya_tuslah { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double? panjang_ori_kenderaan { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double? tinggi_kenderaan { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double? lebar_kenderaan { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
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

        public string? nik_penumpang1 { get; set; }
        public string? nik_penumpang2 { get; set; }
        public string? nik_penumpang3 { get; set; }

        public int? usia_penumpang1 { get; set; }

        public int? usia_penumpang2 { get; set; }

        public int? usia_penumpang3 { get; set; }


    }
}
