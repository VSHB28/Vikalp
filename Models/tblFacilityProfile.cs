namespace Vikalp.Models
{
    public class tblFacilityProfile
    {
        public int ProfileId { get; set; }
        public string ProfileGuid { get; set; }
        public int FacilityId { get; set; }
        public int? SubCenterId { get; set; }
        public int PopulationCoveredByPHC { get; set; }
        public int NumberOfHSC { get; set; }
        public int? PopulationCoveredByHWC { get; set; }
        public int? AverageOPDPerDay { get; set; }
        public int NearestFacilityReferral { get; set; }
        public decimal? DistanceFromPHC { get; set; }
        public int IsDeliveryPoint { get; set; }
        public int? AvgDeliveryPerMonth { get; set; }
        public decimal? DistanceFromDH { get; set; }
        public int IsSeparateSpaceForFp { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
