namespace Vikalp.Models.DTO
{
    public class UserLocationMappingDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int? BlockId { get; set; }
    }

}
