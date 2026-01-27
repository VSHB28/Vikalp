using System.Data;
using Microsoft.Data.SqlClient;
using Vikalp.Models.DTO;
using Vikalp.Utilities;

namespace Vikalp.Service;

public class RoleService : IRoleService
{
    private readonly IConfiguration _config;

    public RoleService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

    public List<RoleDto> GetAll()
    {
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetRoles", null);
        return dt.AsEnumerable().Select(r => new RoleDto
        {
            RoleId = r.Field<int>("RoleId"),
            RoleName = r.Field<string>("RoleName"),
            RoleType = r.Field<string>("RoleType"),
            Description = r.Field<string>("Description"),
            IsActive = r.Field<int>("IsActive")
        }).ToList();
    }

    public RoleDto? GetById(int id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@RoleId", SqlDbType.Int) { Value = id } };
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetRoleById", parameters);
        var row = dt.AsEnumerable().FirstOrDefault();
        if (row == null) return null;
        return new RoleDto
        {
            RoleId = row.Field<int>("RoleId"),
            RoleName = row.Field<string>("RoleName"),
            RoleType = row.Field<string>("RoleType"),
            Description = row.Field<string>("Description"),
            IsActive = row.Field<int>("IsActive")
        };
    }

    public int Create(RoleDto role)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@RoleName", SqlDbType.NVarChar) { Value = role.RoleName },
            new SqlParameter("@RoleType", SqlDbType.Bit) { Value = role.RoleType },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = role.Description },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = role.IsActive }
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_CreateRole", parameters);
        if (dt.Rows.Count > 0 && dt.Columns.Contains("NewId"))
        {
            return Convert.ToInt32(dt.Rows[0]["NewId"]);
        }
        return 0;
    }

    public bool Update(RoleDto role)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@RoleId", SqlDbType.Int) { Value = role.RoleId },
            new SqlParameter("@RoleName", SqlDbType.NVarChar) { Value = role.RoleName },
            new SqlParameter("@RoleType", SqlDbType.Bit) { Value = role.RoleType },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = role.Description },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = role.IsActive }
        };

        SqlUtils.ExecuteSP(Conn(), "dbo.sp_UpdateRole", parameters);
        return true;
    }

    public bool Delete(int id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@RoleId", SqlDbType.Int) { Value = id } };
        SqlUtils.ExecuteSP(Conn(), "dbo.sp_DeleteRole", parameters);
        return true;
    }
}