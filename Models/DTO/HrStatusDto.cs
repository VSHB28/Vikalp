using System.ComponentModel.DataAnnotations;

namespace Vikalp.Models.DTO
{
    public class HrStatusDto
    {
        public int? HrId { get; set; }
        public string HrGuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DesignationId { get; set; }
        public int GenderId { get; set; }


        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numeric values allowed")]
        public string Mobile { get; set; } = string.Empty;
        public int FacilityTypeId { get; set; }
        public int FacilityId { get; set; }
        public int TrainedAntaraGovt { get; set; }
        public int TrainedAntaraIDF { get; set; }
        public int AttendentVCAT { get; set; }
        public int TrainedInIUCD { get; set; }
        public int TrainedInFPLMIS { get; set; }

        public int? TrainedInCACS_MMA { get; set; }

        // Audit fields can be optional in DTOs depending on usage
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
