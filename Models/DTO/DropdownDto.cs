namespace Vikalp.Models.DTO
{
    public class DropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AshaDetailDto
    {
        public int AshaId { get; set; }
        public string MobileNumber { get; set; }
    }

    public class AshaListDto
    {
        public int AshaId { get; set; }
        public int? FacilityId { get; set; }
        public string? AshaName { get; set; }
        public string? AshaMobile { get; set; }
        public string? SubCenter { get; set; }
        public string? FacilityName { get; set; }
    }

    public class ParticipantListDto
    {
        public string? FullName { get; set; }
        public int? FacilityId { get; set; }
        public string? Mobile { get; set; }
        public string? FacilityName { get; set; }
    }
}
