namespace Vikalp.Models.DTO
{
    public class SubCentreProfileDto
    {
        public int? ProfileId { get; set; }     // For Update

        public int SubCenterId { get; set; }    // Foreign Key

        public int PopulationCoveredbyPHC { get; set; }
        public int NumberofHSC { get; set; }

        public int? PopulationCoveredPHC_HWC { get; set; }
        public int? PopulationCoveredbyHWC { get; set; }
        public int? AverageOPDperDay { get; set; }

        public string? NearestFacilityReferral { get; set; }

        public decimal? DistancefromPHC { get; set; }
        public int? IsDeliveryPoint { get; set; }
        public int? AvgdeliveryperMonth { get; set; }
        public decimal? DistancefromDH { get; set; }
        public int? IsSeparateSpaceforFp { get; set; }
    }
}
