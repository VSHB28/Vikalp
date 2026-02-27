using System.ComponentModel.DataAnnotations;


namespace Vikalp.Models.DTO
{
    public class SubCentreDto
    {
        public int? SubCentreId { get; set; }
        public string SubCentre { get; set; }
        [Required(ErrorMessage = "NIN Number is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numeric values allowed")]
        [StringLength(12, ErrorMessage = "Maximum 12 digits allowed")]
        public string? NinNumber { get; set; }
        //public string? NinNumber { get; set; }
        public bool IsLearningSite { get; set; }
        public bool IsActive { get; set; }
        public int? BlockId { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? AnmId { get; set; }
        public string? BlockName { get; set; }
      
        public string? FacilityType { get; set; }
        public int RegionId { get; set; }
        public int DistrictId { get; set; }
    }
}
