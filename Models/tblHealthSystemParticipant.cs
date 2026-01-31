using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vikalp.Models
{
    [Table("tblHealthSystemParticipant")]
    public class tblHealthSystemParticipant
    {
        [Key]
        public int ParticipantId { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public int? ProviderTypeId { get; set; }
        public int? FacilityId { get; set; }

        [MaxLength(200)]
        public string? FacilityName { get; set; }

        public int? FacilityTypeId { get; set; }

        [MaxLength(200)]
        public string? FacilityTypeOther { get; set; }

        public int? InterventionFacility { get; set; }
        public int? DistrictId { get; set; }
        public int? GenderId { get; set; }

        [MaxLength(10)]
        public string? Mobile { get; set; }

        public int? VCATScorePreTest { get; set; }
        public int? VCATScorePostTest { get; set; }
        public int? RefresherTraining { get; set; }

        public string? Remarks { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
