using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
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
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            return RedirectToAction("Login", "Account");

        int userId = int.Parse(claim.Value);

        var roleList = _roleMenuService.GetAll(userId);

        return View(roleList);
    }

    public IActionResult Index1()
    {
        

        return View();
    }

    public JsonResult GetMenusByRole(int roleId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var menus = _roleMenuService.GetParentMenusByRole(roleId, userId);
        return Json(menus);
    }

    [HttpPost]
    public IActionResult SaveRoleMenuMapping([FromBody] RoleMenuSaveDTO model)
    {
        if (model == null)
            return Json(new { success = false, message = "Model is null" });

        var result = _roleMenuService.SaveRoleMenuMapping(model);

        return Json(new { success = result });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnassignMenu([FromBody] UnassignRoleMenuDTO model)
    {
        var result = await _roleMenuService
            .UnassignMenuAsync(model.RoleId, model.MenuId);

        return Json(new { success = result });
    }

}