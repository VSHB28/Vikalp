using System;
using System.ComponentModel.DataAnnotations;

namespace Vikalp.Models.DTO
{
    public class AshaOrientationDto
    {
        public int UID { get; set; }
        public int VenueId { get; set; }
        public string? VenueGuid { get; set; }
        public string? OrientationGuid { get; set; }
        public DateTime DateofOrientation { get; set; }
        public int? AshaId { get; set; }

        public long? NIN { get; set; }
        public int? VCAT_PreTest { get; set; }
        public int? VCAT_PostTest { get; set; }
        public int? IsOrientation { get; set; }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }


        // Asha details
        public string? AshaName { get; set; }
        public string? AshaMobile { get; set; }
        public int IsIntervention { get; set; }

        // Facility details
        public int FacilityId { get; set; }
        public string? FacilityName { get; set; }

        // Block details
        public int BlockId { get; set; }
        public string? BlockName { get; set; }
        public string? BlockCode { get; set; }
        public int RegionId { get; set; }

        // District details
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? DistrictCode { get; set; }

        // State details
        public int StateId { get; set; }
        public string? StateName { get; set; }
        public string? StateCode { get; set; }

        // Orientation venue
        public string? Venue { get; set; }

        // helper flags
        public bool IsEdit { get; set; }   // 👈 key
        public List<int> TopicsCovered { get; set; }
        public int InterventionCount { get; set; }
        public int NonInterventionCount { get; set; } 
        public int TotalOrientations { get; set; }
        public string? VenueName { get; set; }     // Comes from dropdown text or textbox
    }

    public class AshaOrientationCreateDto
    {
        // Location
        public string? VenueGuid { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int BlockId { get; set; }

        // Facility
        public int? FacilityId { get; set; }          // Nullable for non-intervention
        public string? FacilityName { get; set; }     // Comes from dropdown text or textbox
        public string? Venue { get; set; }            // Optional alias if you want
        public int IsIntervention { get; set; }       // 1 = Intervention, 0 = Non-intervention

        public List<int> TopicsCovered { get; set; }   // For edit scenarios

        // Orientation
        [Required(ErrorMessage = "Orientation date is required")]
        public DateTime DateofOrientation { get; set; }

        // Non-intervention
        public long? NIN { get; set; }

        // ASHAs
        public List<AshaOrientationRowDto> Attendees { get; set; } = new();
    }


    public class AshaOrientationRowDto
    {
        public int? AshaId { get; set; }          // Nullable
        public string AshaName { get; set; }      // NOT NULL in table
        public string? AshaMobile { get; set; }
        public string? VenueName { get; set; }     // Comes from dropdown text or textbox
        public int VenueId { get; set; }
        public int? VCAT_PreTest { get; set; }
        public int? VCAT_PostTest { get; set; }

        public int IsOrientation { get; set; }   // Will convert to 1/0
    }

}
