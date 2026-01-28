namespace Vikalp.Models.DTO
{
    public class HealthSystemActivityDto
    {
        public string ActivityName { get; set; }
        public string? OtherActivity { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string? TrainingVenue { get; set; }
        public int? ActivityTypeId { get; set; }
        public int? ActivityFormatId { get; set; }
        public int? NoOfParticipants { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Remarks { get; set; }

    }
}
