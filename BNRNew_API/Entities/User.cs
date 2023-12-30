using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("user")]
    [Index("UserName" ,Name = "user_idx1", IsUnique = true)]
    public class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool? Active { get; set; }

        [Required]
        public string? Role { get; set; }


        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public List<string> roleArray { get; set; }

    }
}
