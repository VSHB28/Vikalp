namespace Vikalp.Models.DTO
{
    public class FacilityDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public bool IsActive { get; set; }
        public string NinNumber { get; set; }
        public int BlockId { get; set; }
        public bool IsIntervention { get; set; }

        public int RegionId { get; set; }   
        public int DistrictId { get; set; }
        public int? ProfileId { get; set; }
        public int? HrId { get; set; }
    }

}
