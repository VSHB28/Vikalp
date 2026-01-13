using System;

namespace Vikalp.Models.DTO
{
    public class AshaOrientationDto
    {
        public int UID { get; set; }
        public string? ANMname { get; set; }
        public int? FacilityId { get; set; }
        public long? FacilityNIN { get; set; }
        public long? Mobile { get; set; }
        public int? VCAT { get; set; }
        public bool? IsOrientation { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
    }
}
