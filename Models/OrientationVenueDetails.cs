namespace Vikalp.Models
{
    public class OrientationVenueDetails
    {
        public int VenueId { get; set; }                // Primary Key
        public string VenueGuid { get; set; } = string.Empty;
        public int IsIntervention { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int BlockId { get; set; }
        public int? FacilityId { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public DateTime DateofOrientation { get; set; }
        public long? NIN { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
