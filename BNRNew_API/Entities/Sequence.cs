using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("sequence")]
    [Index("key" ,Name = "sequence_idx1", IsUnique = true)]
    public class Sequence
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public SequenceKey key { get; set; }

        [Required]
        public int last_value { get; set; }

        [Required]
        public DateTime last_used { get; set; }

    }

    public enum SequenceKey
    {
        TICKET = 1,
        MANIFEST = 2
    }
}
