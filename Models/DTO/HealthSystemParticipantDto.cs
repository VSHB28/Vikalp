namespace Vikalp.Models.DTO
{
    public class HealthSystemParticipantDto
    {
        public DateTime DateofActivity { get; set; }
        public int ParticipantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int StateId { get; set; }
        public int? ProviderTypeId { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? FacilityTypeId { get; set; }
        public string? FacilityTypeOther { get; set; }
        public int? InterventionFacility { get; set; }
        public int? DistrictId { get; set; }
        public int? GenderId { get; set; }

        public string? Mobile { get; set; }
        public int? VCATScorePreTest { get; set; }
        public int? VCATScorePostTest { get; set; }
        public int? RefresherTraining { get; set; }
        public string? Remarks { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }

    public class HealthSystemParticipantSaveDto
    {
        public DateTime DateofActivity { get; set; }
        public int StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? FacilityTypeId { get; set; }
        public string? FacilityTypeOther { get; set; }
        public List<HealthSystemParticipantDto> Participants { get; set; } = new();
    }  

}
