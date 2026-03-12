namespace Vikalp.Models.DTO
{
    public class tblAshaDetailsDto
    {
        public int AshaDetailId { get; set; }                // Identity column
        public string AshaDetailGuid { get; set; }           // Primary Key (nvarchar 100)

        public int? FacilityId { get; set; }                 // Nullable int
        public int? SubCentreId { get; set; }                // Nullable int
        public string AshaName { get; set; }                 // nvarchar(150)
        public string MobileNo { get; set; }                 // nvarchar(20)

        public int? VcatAttended { get; set; }               // Nullable int
        public int? IsActive { get; set; }                   // Nullable int
        public int? PostedSince { get; set; }                // Nullable int
        public int? IsEdited { get; set; }                   // Nullable int

        public int? CreatedBy { get; set; }                  // Nullable int
        public DateTime? CreatedOn { get; set; }             // Nullable datetime
        public int? UpdatedBy { get; set; }                  // Nullable int
        public DateTime? UpdatedOn { get; set; }             // Nullable datetime
    }
}
