using Vikalp.DTO;
using Vikalp.Models.DTO;

public interface IRoleMenuService
{
    List<RoleMenuDTO> GetAll(int userId);
    List<MenuDropdownDTO> GetParentMenusByRole(int roleId, int userId);
    bool SaveRoleMenuMapping(RoleMenuSaveDTO model);
    Task<bool> UnassignMenuAsync(int roleId, int menuId);
}
