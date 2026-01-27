using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("LocationStates")]
    public class LocationStates
    {
        [Key]
        public int StateId { get; set; }

        [StringLength(50)]
        public string? StateName { get; set; }

        [StringLength(50)]
        public string? StateCode { get; set; }

        public int? IsActive { get; set; }

        // Navigation
        public List<LocationDistricts> Districts { get; set; } = new();
    }
}
