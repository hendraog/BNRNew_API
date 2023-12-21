using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
    public class CreateCargoManifestRequest
    {
        public long? id { get; set; }

        [Required]
        public DateTime? tgl_manifest { get; set; }

        [Required]
        public string nama_kapal { get; set; }

        [Required]
        public string nama_nahkoda { get; set; }
        [Required]
        public string lokasi_asal { get; set; }
        [Required]
        public string lokasi_tujuan { get; set; }
        [Required]
        public string alamat { get; set; }

        [Required]
        public List<long> ticketList { get; set; }

    }
}