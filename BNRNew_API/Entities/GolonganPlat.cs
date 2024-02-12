using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("golongan_plat")]
    [Index("plat_no" ,Name = "golongan_plat_idx1", IsUnique = true)]
    public class GolonganPlat
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string plat_no { get; set; }

        [Required]
        public long golonganid { get; set; }


        [ForeignKey(nameof(golonganid))]
        [Required]
        public Golongan golongan { get; set; }


        [Required]
        public long CreatedBy { get; set; }


        [ForeignKey(nameof(CreatedBy))]

        public User? CreatedByData { get; set; }

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
