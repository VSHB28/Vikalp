using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlTypes;
using Vikalp.Models.DTO;



public class RoleMenuService : IRoleMenuService
{
    private readonly IConfiguration _configuration;

    public RoleMenuService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
    public List<RoleMenuDTO> GetAllMenus(int userId)
    {
        try
        {
            using (var connection = GetConnection())
            {
                return connection.Query<RoleMenuDTO>(
                    "sp_GetRolesMenuList",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }
        catch (Exception ex)
        {
            throw;
        }       
    }

    public List<RoleMenuDTO> GetParentMenusByRole(int roleId, int userId)
    {
        using var con = GetConnection();
        return con.Query<RoleMenuDTO>(
            "sp_GetRolesMenuListbyrolid",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure
        ).ToList();
    }

    public bool SaveRoleMenuMapping(RoleMenuSaveDTO model)
    {
        using var con = GetConnection();
        con.Open();

        using var transaction = con.BeginTransaction();

        try
        {
            // Delete old mappings
            con.Execute(
                "sp_SaveRoleMenuMapping",
                new { Action = "DELETE_BY_ROLE", RoleId = model.RoleId },
                commandType: CommandType.StoredProcedure,
                transaction: transaction
            );

            // Insert new mappings
            if (model.MenuIds != null && model.MenuIds.Any())
            {
                foreach (var menuId in model.MenuIds)
                {
                    con.Execute(
                        "sp_SaveRoleMenuMapping",
                        new
                        {
                            Action = "INSERT",
                            RoleId = model.RoleId,
                            MenuId = menuId
                        },
                        commandType: CommandType.StoredProcedure,
                        transaction: transaction
                    );
                }
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> UnassignMenuAsync(int roleId, int menuId)
    {
        try
        {
            using var con = GetConnection();
            {
                using (SqlCommand cmd = new SqlCommand("sp_UnassignRoleMenu", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.Parameters.AddWithValue("@MenuId", menuId);

                    await con.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    return Convert.ToInt32(result) == 1;
                }
            }
        }
        catch
        {
            return false;
        }
    }
}
