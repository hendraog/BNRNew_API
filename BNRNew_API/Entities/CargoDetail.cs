using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Table("cargo_detail")]
    public class CargoDetail 
    {

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? id { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(cargoManifestid))]
        public CargoManifest cargoManifest { get; set; }

        [JsonIgnore]
        public long? cargoManifestid { get; set; }

        [Required]
        public long? ticket { get; set; }
        
        [Required]
        public long CreatedBy { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public string ticketNo { get; set; }

    }
}
