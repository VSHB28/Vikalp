namespace Vikalp.Models.DTO
{
    public class EventActivityDto
    {
        public int EventId { get; set; }
        public string EventGuid { get; set; }
        public int? EventTypeId { get; set; }
        public DateTime? EventDate { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public int? FacilityId { get; set; }
        public int? SubcentreId { get; set; }
        public string VillageName { get; set; }

        public int? Women_0Children { get; set; }
        public int? Men_0Children { get; set; }
        public int? Women_1Child { get; set; }
        public int? Men_1Child { get; set; }
        public int? Women_2PlusChildren { get; set; }
        public int? Men_2PlusChildren { get; set; }

        public int? MotherInLawCount { get; set; }
        public int? IPCSessionHeld { get; set; }
        public int? HappinessKitDistributed { get; set; }

        public int? WomenReferred_TemporaryMethods { get; set; }
        public int? MenReferred_TemporaryMethods { get; set; }
        public int? WomenAdopted_Parity0 { get; set; }
        public int? WomenAdopted_Parity1 { get; set; }

        public int? AntaraGiven_Parity0 { get; set; }
        public int? AntaraGiven_Parity1 { get; set; }
        public int? AntaraDosesGiven { get; set; }

        public int? HelplineCalls { get; set; }
        public int? AntaraLeadsSent { get; set; }

        public int? LeafletsDistributed { get; set; }
        public int? AntaraLeafletCount { get; set; }
        public int? NPKLeafletCount { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        // SubCenter details
        public string? SubCenter { get; set; }

        // Facility details
        public string? FacilityName { get; set; }

        // Block details
        public string? BlockName { get; set; }

        // District details
        public string? DistrictName { get; set; }

        // State details
        public string? StateName { get; set; }
    }
}
