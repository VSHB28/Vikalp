namespace Vikalp.Models
{
    public class MstFacility
    {
        public int FacilityId { get; set; }                 // Primary Key
        public string FacilityName { get; set; } = string.Empty;
        public int FacilityType { get; set; }
        public int IsActive { get; set; }
        public long? NinNumber { get; set; }                // bigint → long?
        public int BlockId { get; set; }
        public int IsIntervention { get; set; }
        public int? ProfileId { get; set; }
    }
}
