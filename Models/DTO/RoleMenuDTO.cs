namespace Vikalp.Models.DTO
{

    public class RoleMenuDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public string? Icon { get; set; }
        public string? MenuIds { get; set; }
        public string? MenuNames { get; set; }
    }

    public class RoleMenuSaveDTO
    {
        public int RoleId { get; set; }

        public List<int> MenuIds { get; set; }
    }

    public class RoleMenuMappingDTO
    {
        public int ParentMenuId { get; set; }

        public List<int> ChildMenuIds { get; set; }
    }

    public class MenuDropdownDTO
    {
        public int MenuId { get; set; }
        public string? MenuName { get; set; }

        public int RoleMenuId { get; set; }

    }

    public class UnassignRoleMenuDTO
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }

}
