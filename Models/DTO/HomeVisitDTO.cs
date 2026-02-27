using System;
namespace Vikalp.Models.DTO
{

    public class HomeVisitDTO
    {
        // Line Listing fields
        public int LineListId { get; set; }
        public string LineListGuid { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public int? BlockId { get; set; }
        public string? BlockName { get; set; }
        public string? VillageName { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? SubCenterId { get; set; }
        public string? SubCentre { get; set; }
        public int? ASHAId { get; set; }
        public string? AnganwadiWorkerName { get; set; }
        public string? WomanName { get; set; }
        public string? HusbandName { get; set; }
        public string? MobileNumber { get; set; }

        // Home Visit fields
        public int? VisitId { get; set; }
        public string? VisitGuid { get; set; }
        public string? ASHAName { get; set; }
        public int? ClientParity { get; set; }
        public int? IsCurrentlyPregnant { get; set; }
        public int? IsReceivingSocialBenefit { get; set; }
        public List<int>? SocialBenifits { get; set; }
        public int? IsUsingFamilyPlanning { get; set; }
        public int? FamilyPlanningMethod { get; set; }
        public int? IsCounsellingDone { get; set; }
        public int? IsReferredForServices { get; set; }
        public int? ReferredHealthCenterType { get; set; }
        public string? ReferredHealthCenterName { get; set; }
        public int? IsConsentTaken { get; set; }
        public int? IsCallInitiated { get; set; }

        // Audit fields
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        // Extra naming consistency
        public string? SubCenterName { get; set; }

        public string? HomeVisit { get; set; }

        public int UserId { get; set; }

        public List<HomevisitFollowUpDto> FollowUpHistory { get; set; }
        = new List<HomevisitFollowUpDto>();

    }

    public class HomevisitFollowUpDto
    {
        public string? WomanName { get; set; }
        public string? HusbandName { get; set; }
        public string? MobileNumber { get; set; }
        public int? FollowupId { get; set; }
        public Guid FollowupGuId { get; set; }
        public Guid? LineListGuid { get; set; }
        public Guid? HomeVistGuid { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string? Remark { get; set; }
        public int? FollowupStatus { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsEdited { get; set; }
    }

}


