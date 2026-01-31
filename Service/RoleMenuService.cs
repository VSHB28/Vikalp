using System.Data;
using Microsoft.Data.SqlClient;
using Vikalp.Models.DTO;
using System.Data.SqlTypes;
using Dapper;



public class RoleMenuService : IRoleMenuService
{
    private readonly IConfiguration _configuration;

    public RoleMenuService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));
    }


    public List<RoleMenuDTO> GetAll()
    {
        using (var connection = GetConnection())
        {
            return connection.Query<RoleMenuDTO>(
                "USP_RoleMenu_Master",
                new { Action = "SELECT" },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
    }

    public List<MenuDropdownDTO> GetParentMenusByRole(int roleId)
    {
        using var con = GetConnection();
        return con.Query<MenuDropdownDTO>(
            "USP_RoleMenu_Master",
            new { Action = "GET_PARENT_MENUS", RoleId = roleId },
            commandType: CommandType.StoredProcedure
        ).ToList();
    }

    public List<MenuDropdownDTO> GetChildMenus(int parentMenuId, int roleId)
    {
        using var con = GetConnection();
        return con.Query<MenuDropdownDTO>(
            "USP_RoleMenu_Master",
            new
            {
                Action = "GET_CHILD_MENUS",
                ParentMenuId = parentMenuId,
                RoleId = roleId
            },
            commandType: CommandType.StoredProcedure
        ).ToList();
    }

    public List<MenuDropdownDTO> GetAllParentMenus()
    {
        using var con = GetConnection();
        return con.Query<MenuDropdownDTO>(
            @"SELECT MenuId, MenuName 
          FROM Menus 
          WHERE ParentMenuId IS NULL 
          ORDER BY SortOrder"
        ).ToList();
    }

    public List<MenuDropdownDTO> GetAllChildMenus(int parentMenuId)
    {
        using var con = GetConnection();
        return con.Query<MenuDropdownDTO>(
            @"SELECT MenuId, MenuName 
          FROM Menus 
          WHERE ParentMenuId = @ParentMenuId 
          ORDER BY SortOrder",
            new { ParentMenuId = parentMenuId }
        ).ToList();
    }

}
