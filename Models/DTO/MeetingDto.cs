namespace Vikalp.Models.DTO
{
    public class MeetingDto
    {
        public int MeetingId { get; set; }                     // Identity column
        public string MeetingGuid { get; set; }                // Primary Key (nvarchar 100)

        public int? StateId { get; set; }                      // Nullable int
        public int? DistrictId { get; set; }                   // Nullable int
        public int? BlockId { get; set; }                      // Nullable int
        public int? FacilityId { get; set; }                     // Nullable int

        public int? MeetingType { get; set; }                  // Nullable int
        public DateTime? MeetingDate { get; set; }             // Nullable date
        public string MeetingVenue { get; set; }               // nvarchar(250)        
        public string Participants { get; set; }               // nvarchar(200)
        public string ParticipantOther { get; set; }           // nvarchar(200)

        public int? ReversibleMethodsReview { get; set; }      // Nullable int
        public int? AshaIncentivesReview { get; set; }         // Nullable int
        public int? FplmisAndStockStatusReview { get; set; }   // Nullable int
        public int? StaffFacilitateReversibleContraceptives { get; set; } // Nullable int
        public int? GovernmentOfficialJointVisit { get; set; } // Nullable int

        public string IdfStaffName { get; set; }               // nvarchar(150)
        public string Remarks { get; set; }                    // nvarchar(500)

        public int? CreatedBy { get; set; }                    // Nullable int
        public DateTime? CreatedOn { get; set; }               // Nullable datetime
        public int? UpdatedBy { get; set; }                    // Nullable int
        public DateTime? UpdatedOn { get; set; }               // Nullable datetime
        public int? MeetingPlatform { get; set; }
        public int? IsIDFstaffPresent { get; set; }
        public string? MeetingBlockParticipant { get; set; }
        public string? StateName { get; set; }
        public string? DistrictName { get; set; }
        public string? BlockName { get; set; }
        public string? FacilityName { get; set; }
    }
}
