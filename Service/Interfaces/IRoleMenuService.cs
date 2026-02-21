using Vikalp.DTO;
using Vikalp.Models.DTO;

public interface IRoleMenuService
{
    List<RoleMenuDTO> GetAllMenus(int userId);
    List<RoleMenuDTO> GetParentMenusByRole(int roleId, int userId);
    bool SaveRoleMenuMapping(RoleMenuSaveDTO model);
    Task<bool> UnassignMenuAsync(int roleId, int menuId);
}
