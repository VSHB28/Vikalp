using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("tblAshaOrientation")]
    public class TblAshaOrientation
    {
        [Key]
        public int UID { get; set; }                      // Primary Key
        public string VenueGuid { get; set; } = string.Empty;
        public string OrientationGuid { get; set; } = string.Empty;
        public int IsIntervention { get; set; }

        public int? AshaId { get; set; }
        public string AshaName { get; set; } = string.Empty;
        public string? AshaMobile { get; set; }

        public int? FacilityId { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public DateTime DateofOrientation { get; set; }
        public long? NIN { get; set; }
        public int? VCAT_PreTest { get; set; }
        public int? VCAT_PostTest { get; set; }
        public int? IsOrientation { get; set; }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

        public int StateId { get; set; }
        public string? StateName { get; set; }

        public int DistrictId { get; set; }

        public string? DistrictName { get; set; }

        public int BlockId { get; set; }

        public string? BlockName { get; set; }
        public string? Venue { get; set; }
    }
}
