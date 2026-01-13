using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models;
using Vikalp.Utilities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Vikalp.Service;

public class MenuService
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MenuService> _logger;

    public MenuService(IConfiguration config, IHttpContextAccessor httpContextAccessor, ILogger<MenuService> logger)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public List<MenuViewModel> GetMenusByRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        _logger.LogInformation("GetMenusByRole called for user {User}", user?.Identity?.Name ?? "anonymous");

        var roleClaim = user?.FindFirst(ClaimTypes.Role);

        int? roleId = null;
        if (roleClaim != null && int.TryParse(roleClaim.Value, out var parsed))
        {
            roleId = parsed;
        }

        _logger.LogInformation("Resolved roleId = {RoleId}", roleId?.ToString() ?? "null");

        var conn = _config.GetConnectionString("DefaultConnection");

        SqlParameter[] parameters =
        {
            new SqlParameter("@RoleId", roleId ?? (object)DBNull.Value)
        };

        var dt = SqlUtils.ExecuteSP(
            conn,
            "dbo.sp_GetMenuByRole",
            parameters
        );

        _logger.LogInformation("Stored procedure returned {Rows} rows", dt.Rows.Count);

        var flatMenus = dt.AsEnumerable().Select(r => new MenuViewModel
        {
            MenuId = r.Field<int>("MenuId"),
            MenuName = r.Field<string>("MenuName"),
            ParentMenuId = r.Field<int?>("ParentMenuId"),
            Url = r.Field<string>("Url"),
            MenuIcon = r.Field<string>("MenuIcon")
        }).ToList();

        // Build hierarchy
        var lookup = flatMenus.ToLookup(m => m.ParentMenuId);
        foreach (var menu in flatMenus)
            menu.Children = lookup[menu.MenuId].ToList();

        return lookup[null].ToList();
    }
}