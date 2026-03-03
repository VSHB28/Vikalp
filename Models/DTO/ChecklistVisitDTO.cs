namespace Vikalp.Models.DTO
{
    public class ChecklistVisitDTO
    {
        public int ChecklistId { get; set; }
        public string? ChecklistGuid { get; set; }
        public DateTime? VisitDate { get; set; }
        public int VisitType { get; set; }
        public int? VisitorDesignation { get; set; }
        public string? VisitorName { get; set; }
        public string? OtherVisitorName { get; set; }

        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public int? BlockId { get; set; }
        public string? BlockName { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? SubcentreId { get; set; }
        public string? SubCentre { get; set; }

        // District
        public int? DistrictOfficialsOriented { get; set; }
        public int? DistrictOfficialsHMProtocol { get; set; }
        public int? DistrictNoStockOut { get; set; }
        public int? DistrictASHAFelicitated { get; set; }
        public int? DistrictELAsIssued { get; set; }
        public int? DistrictReviewsConducted { get; set; }
        public int? DistrictReviewsInDHS { get; set; }
        public int? DistrictFollowupLetterIssued { get; set; }

        // Block
        public int? BlockOfficialsOriented { get; set; }
        public int? BlockOfficialsHMProtocol { get; set; }
        public int? BlockDashboardUsed { get; set; }
        public int? BlockDataEntryCompleted { get; set; }
        public int? BlockNoStockOut { get; set; }
        public int? BlockASHAFelicitated { get; set; }
        public int? BlockELAsIssued { get; set; }

        // PHC
        public int? PHCTrainedProviderAvailable { get; set; }
        public int? PHCExplainerVideoUsed { get; set; }
        public int? PHCIECMaterialsAvailable { get; set; }
        public int? PHCSelfcareKitsInstalled { get; set; }
        public int? PHCRecordsUpdated { get; set; }
        public int? PHCNoStockOut { get; set; }
        public int? PHCDashboardUsed { get; set; }
        public int? PHCMOProtocolArticulated { get; set; }
        public int? PHCSPProtocolArticulated { get; set; }
        public int? PHCELAsDisseminated { get; set; }
        public int? PHCASHATrained { get; set; }

        // HWC
        public int? HWCTrainedProviderAvailable { get; set; }
        public int? HWCExplainerVideoUsed { get; set; }
        public int? HWCIECMaterialsAvailable { get; set; }
        public int? HWCSelfcareKitsInstalled { get; set; }
        public int? HWCRecordsUpdated { get; set; }
        public int? HWCNoStockOut { get; set; }
        public int? HWCDashboardUsed { get; set; }
        public int? HWCAntaraDosesProvided { get; set; }
        public int? HWCELAsDisseminated { get; set; }
        public int? HWCASHATrained { get; set; }
    }
}
