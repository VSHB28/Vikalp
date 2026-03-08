using System.ComponentModel.DataAnnotations;

namespace Vikalp.Models.DTO
{
    public class FacilityProfileDto
    {
        public int? ProfileId { get; set; }
        public string ProfileGuid { get; set; }
        public int FacilityTypeId { get; set; }
        public int FacilityId { get; set; }
        public int? SubCenterId { get; set; }

        [Required(ErrorMessage = "Population is required")]
        [Range(0, 99999, ErrorMessage = "Maximum 5 digits allowed")]
        public int PopulationCoveredbyPHC { get; set; }
        public int NumberofHSC { get; set; }

        [Required(ErrorMessage = "Population is required")]
        [Range(0, 99999, ErrorMessage = "Maximum 5 digits allowed")]
        public int? PopulationCoveredPHC_HWC { get; set; }

        [Required(ErrorMessage = "Population is required")]
        [Range(0, 99999, ErrorMessage = "Maximum 5 digits allowed")]


        public int? PopulationCoveredbyHWC { get; set; }
        public int? AverageOPDperDay { get; set; }
        public string? NearestFacilityReferral { get; set; }

        [RegularExpression(@"^\d{0,2}(\.\d{0,2})?$", ErrorMessage = "Distance must be max 2 digits before and after decimal.")]
        [Range(0, 99.99, ErrorMessage = "Value must be between 0 and 99.99")]
        public decimal? DistancefromPHC { get; set; }
        public int IsDeliveryPoint { get; set; }
        public int? AvgdeliveryperMonth { get; set; }

        [RegularExpression(@"^\d{0,2}(\.\d{0,2})?$", ErrorMessage = "Distance must be max 2 digits before and after decimal.")]
        [Range(0, 99.99, ErrorMessage = "Value must be between 0 and 99.99")]
        public decimal? DistancefromDH { get; set; }
        public int IsSeparateSpaceforFp { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
