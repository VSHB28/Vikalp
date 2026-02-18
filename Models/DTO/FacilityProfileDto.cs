namespace Vikalp.Models.DTO
{
    public class FacilityProfileDto
    {
        public int? ProfileId { get; set; }
        public string ProfileGuid { get; set; }
        public int FacilityTypeId { get; set; }
        public int FacilityId { get; set; }
        public int? SubCenterId { get; set; }
        public int PopulationCoveredbyPHC { get; set; }
        public int NumberofHSC { get; set; }
        public int? PopulationCoveredPHC_HWC { get; set; }
        public int? PopulationCoveredbyHWC { get; set; }
        public int? AverageOPDperDay { get; set; }
        public string? NearestFacilityReferral { get; set; }
        public decimal? DistancefromPHC { get; set; }
        public int IsDeliveryPoint { get; set; }
        public int? AvgdeliveryperMonth { get; set; }
        public decimal? DistancefromDH { get; set; }
        public int IsSeparateSpaceforFp { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
