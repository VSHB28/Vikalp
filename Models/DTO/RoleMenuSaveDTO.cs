namespace Vikalp.Models.DTO
{
    public class RoleMenuSaveDTO
    {
        public int RoleId { get; set; }
        public List<RoleMenuMappingDTO> Mappings { get; set; }
    }

}
