namespace Vikalp.Models.DTO
{

    public class RoleMenuDTO
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? ParentMenuId { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public int IsActive { get; set; }
        public string MenuIcon { get; set; }
        public int Assigned { get; set; }
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
