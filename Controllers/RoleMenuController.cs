using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Vikalp.DTO;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

public class RoleMenuController : Controller
{
    private readonly IRoleMenuService _roleMenuService;
    private readonly IDropdownService _dropdownService;  

    public RoleMenuController(IRoleMenuService roleMenuService, IDropdownService dropdownService)
    {
        _roleMenuService = roleMenuService;
        _dropdownService = dropdownService;  
    }

    public IActionResult Index()
    {
       
        var roleList = _dropdownService.GetRoles();

        ViewBag.RoleList = new SelectList(roleList, "Id", "Name");
        ViewBag.MenuList = new SelectList(new List<SelectListItem>());

        return View();
    }


    [HttpGet]
    public IActionResult GetMenusByRole(int roleId)
    {
        var menus = _roleMenuService.GetParentMenusByRole(roleId);
        return Json(menus);
    }

    public IActionResult GetChildMenus(int parentMenuId, int roleId)
    {
        var menus = _roleMenuService.GetChildMenus(parentMenuId, roleId);
        return Json(menus);
    }


    [HttpGet]
    public IActionResult GetAllMenus()
    {
        var menus = _roleMenuService.GetAllParentMenus();
        return Json(menus);
    }

    [HttpGet]
    public IActionResult GetAllChildMenus(int parentMenuId)
    {
        var menus = _roleMenuService.GetAllChildMenus(parentMenuId);
        return Json(menus);
    }

    [HttpPost]
    public IActionResult SaveRoleMenuMapping( RoleMenuSaveDTO model)
    {
        if (model == null || model.RoleId == 0)
            return Json(new { success = false });

        List<int> menuIds = new();

        foreach (var item in model.Mappings)
        {
            menuIds.Add(item.ParentMenuId);

            if (item.ChildMenuIds != null)
                menuIds.AddRange(item.ChildMenuIds);
        }

        string csv = string.Join(",", menuIds.Distinct());

        using (var con = new SqlConnection(
            HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()
            .GetConnectionString("DefaultConnection")))
        {
            var command = con.CreateCommand();
            command.CommandText = "USP_RoleMenu_Master";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "SAVE_MAPPING");
            command.Parameters.AddWithValue("@RoleId", model.RoleId);
            command.Parameters.AddWithValue("@MenuIds", csv);

            con.Open();
            command.ExecuteNonQuery();
        }

        return Json(new { success = true });
    }


}
