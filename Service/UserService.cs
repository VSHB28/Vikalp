using Microsoft.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using Vikalp.Data;
using Vikalp.Helpers;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Vikalp.Services;
using Vikalp.Utilities;

namespace Vikalp.Service;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _db;
    private readonly JwtTokenHelper _jwtHelper;
    private readonly ILogger<UserService> _logger;

    public UserService(IConfiguration config, ApplicationDbContext db, ILogger<UserService> logger, JwtTokenHelper jwtHelper)
    {
        _config = config;
        _db = db;
        _jwtHelper = jwtHelper;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

    public async Task<string> GetUsersAsync()
    {
        return await _db.ExecuteJsonAsync("SELECT sp_get_users()");
    }

    public List<UserDto> GetAll()
    {
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetUsers", null);
        return dt.AsEnumerable().Select(r => new UserDto
        {
            // read UserId as Guid
            UserId = r.Field<int>("UserId"),
            Username = r.Field<string>("Username"),
            Email = r.Field<string>("Email"),
            RoleId = r.Field<int>("RoleId"),
            IsActive = r.Field<bool>("IsActive")
        }).ToList();
    }

    public UserDto? GetById(Guid id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = id } };
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetUserById", parameters);
        var row = dt.AsEnumerable().FirstOrDefault();
        if (row == null) return null;
        return new UserDto
        {
            UserId = row.Field<int>("UserId"),
            Username = row.Field<string>("Username"),
            Email = row.Field<string>("Email"),
            RoleId = row.Field<int>("RoleId"),
            IsActive = row.Field<bool>("IsActive")
        };
    }

    public Guid Create(UserDto user)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username },
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
            new SqlParameter("@Password", SqlDbType.NVarChar) { Value = (object?)user.Password ?? DBNull.Value },
            new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier) { Value = (object?)user.RoleId ?? DBNull.Value },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive }
        };

        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_CreateUser", parameters);
        // Expect stored proc to return NewId as uniqueidentifier
        if (dt.Rows.Count > 0 && dt.Columns.Contains("NewId"))
        {
            var raw = dt.Rows[0]["NewId"];
            if (raw is Guid g) return g;
            if (Guid.TryParse(Convert.ToString(raw), out var parsed)) return parsed;
        }

        return Guid.Empty;
    }

    public bool Update(UserDto user)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = user.UserId },
            new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username },
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email },
            new SqlParameter("@Password", SqlDbType.NVarChar) { Value = (object?)user.Password ?? DBNull.Value },
            new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier) { Value = (object?)user.RoleId ?? DBNull.Value },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive }
        };

        SqlUtils.ExecuteSP(Conn(), "dbo.sp_UpdateUser", parameters);
        return true;
    }

    public bool Delete(Guid id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = id } };
        SqlUtils.ExecuteSP(Conn(), "dbo.sp_DeleteUser", parameters);
        return true;
    }
}
