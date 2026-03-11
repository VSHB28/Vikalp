namespace Vikalp.Models.DTO
{
    public class AshaDto
    {
        public int AshaId { get; set; }

        public string AshaName { get; set; }

        public string AshaMobile { get; set; }

        public bool IsActive { get; set; }

        public bool AttendedVCAT { get; set; }

        public bool IsIntervention { get; set; } = false;

        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? BlockId { get; set; }
        public int? FacilityId { get; set; }
        public int? SubCentreId { get; set; }
    }
}
