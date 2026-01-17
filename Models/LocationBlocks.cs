using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("LocationBlocks")]
    public class LocationBlocks
    {
        [Key]
        public int BlockId { get; set; }

        [Required]
        [StringLength(50)]
        public string BlockName { get; set; } = null!;

        [StringLength(50)]
        public string? BlockCode { get; set; }

        public bool IsActive { get; set; }

        public int? RegionId { get; set; }

        [Required]
        public int DistrictId { get; set; }

        public bool IsAspirational { get; set; }
    }
}
