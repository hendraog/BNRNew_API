using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("cargo_manifest")]
    [Index("manifest_no", Name = "manifest_idx1", IsUnique = true)]
    public class CargoManifest
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string manifest_no { get; set; }

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
        public long CreatedBy { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
