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
    }
}
