using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("LocationDistricts")]
    public class LocationDistricts
    {
        [Key]
        public int DistrictId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        [StringLength(50)]
        public string DistrictName { get; set; } = null!;

        [StringLength(50)]
        public string? DistrictCode { get; set; }

        public bool IsActive { get; set; }

        public bool IsCentinalDistrict { get; set; }

        // Navigation
        public List<LocationBlocks> Blocks { get; set; } = new();
    }
}
