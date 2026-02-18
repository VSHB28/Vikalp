namespace Vikalp.Models.DTO
{
    public class LineListingSurveyDto
    {
        public int LineListId { get; set; }
        public string LineListGuid { get; set; }

        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public string? VillageName { get; set; }

        public int? FacilityId { get; set; }
        public int? SubCenterId { get; set; }
        public int? ASHAId { get; set; }
        public string? AnganwadiWorkerName { get; set; }

        public string WomanName { get; set; } = null!;
        public string? HusbandName { get; set; }
        public string? MobileNumber { get; set; }

        public int? IsChildAvailable { get; set; }
        public int? ChildGender { get; set; }
        public DateTime? ChildDOB { get; set; }
        public int? ChildAge { get; set; }
        public DateTime? MarriageDate { get; set; }

        public int? IsCurrentlyPregnant { get; set; }
        public int? IsUsingFamilyPlanning { get; set; }
        public int? FamilyPlanningMethod { get; set; } // comma-separated

        public int? IsAwareOfAntara { get; set; }
        public string? SelectedMethodName { get; set; }
        public string? ReasonForNonUsage { get; set; }

        public int? IsConcent { get; set; }
        public DateTime? ConcentDate { get; set; }
        public string? Signature { get; set; }

        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

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
        public List<LineListingWomanDto> Women { get; set; } = new();
    }

    public class LineListingSurveyCreateDto
    {
        public string LineListGuid { get; set; } = Guid.NewGuid().ToString();

        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public string? VillageName { get; set; }

        public int? FacilityId { get; set; }
        public int? SubCenterId { get; set; }
        public int? ASHAId { get; set; }
        public string? AnganwadiWorkerName { get; set; }

        public List<LineListingWomanDto> Women { get; set; } = new();

        public int? CreatedBy { get; set; }

        // SubCenter details

        public string? SubCenterName { get; set; }

        // Facility details
        public string? FacilityName { get; set; }

        // Block details
        public string? BlockName { get; set; }

        // District details
        public string? DistrictName { get; set; }

        // State details
        public string? StateName { get; set; }
    }

    public class LineListingWomanDto
    {
        public string WomanName { get; set; } = null!;
        public string? HusbandName { get; set; }
        public string? MobileNumber { get; set; }

        public int? IsChildAvailable { get; set; }
        public int? ChildGender { get; set; }
        public DateTime? ChildDOB { get; set; }
        public DateTime? MarriageDate { get; set; }
        public int? ChildAge { get; set; }
        public int? IsCurrentlyPregnant { get; set; }
        public int? IsUsingFamilyPlanning { get; set; }

        // From popup checkboxes
        public int? FamilyPlanningMethod { get; set; }

        public int? IsAwareOfAntara { get; set; }
        public string? SelectedMethodName { get; set; }
        public string? ReasonForNonUsage { get; set; }

        public int? IsConcent { get; set; }
        public DateTime? ConcentDate { get; set; }
        public string? Signature { get; set; }
    }

    public class LineListingConsentDto
    {
        public string LineListGuid { get; set; }

        // Auto-filled (readonly)
        public string WomanName { get; set; }
        public string? HusbandName { get; set; }
        public string? MobileNumber { get; set; }
         public int MobileHandledBy { get; set; }
        // Editable
        public int? IsConcent { get; set; }
        public DateTime? ConcentDate { get; set; }
        public string? Signature { get; set; }

        public int? UpdatedBy { get; set; }

        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public int? SubCenterId { get; set; }
        public int? ASHAId { get; set; }
        public string? VillageName { get; set; }
        public int GenderId { get; set; }
    }

    public class LineListingSurveyUpdateDto
    {
        public string LineListGuid { get; set; } = Guid.NewGuid().ToString();
        public int LineListId { get; set; }

        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public string? VillageName { get; set; }

        public int? FacilityId { get; set; }
        public int? SubCenterId { get; set; }
        public int? ASHAId { get; set; }
        public string? AnganwadiWorkerName { get; set; }

        public List<LineListingWomanDto> Women { get; set; } = new();

        public int? CreatedBy { get; set; }

        // SubCenter details

        public string? SubCenterName { get; set; }

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
