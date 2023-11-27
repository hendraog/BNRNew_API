using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("golongan")]
    [Index("golongan" ,Name = "golongan_idx1", IsUnique = true)]
    public class Golongan
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string golongan { get; set; }

        [Required]
        public double? harga { get; set; }

        [Required]
        public double? min_length { get; set; }

        [Required]
        public double? max_length { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
