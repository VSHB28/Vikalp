namespace Vikalp.Models.DTO
{
    public class OrientationVenueDetailsDto
    {
        public int VenueId { get; set; }
        public string? VenueGuid { get; set; }
        public int IsIntervention { get; set; }

        public int StateId { get; set; }
        public string? StateName { get; set; }   // optional: if you join with LocationStates

        public int DistrictId { get; set; }
        public string? DistrictName { get; set; } // optional: if you join with LocationDistricts

        public int BlockId { get; set; }
        public string? BlockName { get; set; }    // optional: if you join with LocationBlocks

        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }

        public DateTime DateofOrientation { get; set; }
        public long? NIN { get; set; }

        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
