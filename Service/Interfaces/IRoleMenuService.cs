using Vikalp.DTO;
using Vikalp.Models.DTO;

public interface IRoleMenuService
{
    List<RoleMenuDTO> GetAll();
    List<MenuDropdownDTO> GetParentMenusByRole(int roleId);
    List<MenuDropdownDTO> GetChildMenus(int parentMenuId, int roleId);
    List<MenuDropdownDTO> GetAllParentMenus();
    List<MenuDropdownDTO> GetAllChildMenus(int parentMenuId);


}
