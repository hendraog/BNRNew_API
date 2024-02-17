using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("kapal")]
    [Index("nama_kapal", Name = "kapal_idx1", IsUnique = true)]
    public class Kapal
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string kode_kapal { get; set; }

        [Required]
        public string nama_kapal { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User? CreatedByData { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
