namespace Vikalp.Models
{
    public class tblLineListingSurvey
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
        public int? MarriageMonth { get; set; }
        public int? MarriageYear { get; set; }
        public int? IsCurrentlyPregnant { get; set; }
        public int? IsUsingFamilyPlanning { get; set; }
        public string? FamilyPlanningMethod { get; set; } // comma-separated

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

        public int? IsEdited { get; set; }

    }
}
