namespace Vikalp.Models.DTO
{
    public class RoleMenuMappingDTO
    {
        public int ParentMenuId { get; set; }
        public List<int> ChildMenuIds { get; set; }
    }

}
