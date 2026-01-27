namespace Vikalp.DTO
{
    public class LocationDistrictDTO
    {
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsCentinalDistrict { get; set; }
    }
}
