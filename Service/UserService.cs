using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _db;
    private readonly JwtTokenHelper _jwtHelper;
    private readonly ILogger<UserService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IConfiguration config, ApplicationDbContext db, ILogger<UserService> logger, JwtTokenHelper jwtHelper, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _db = db;
        _jwtHelper = jwtHelper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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
            UserId = r.Field<int>("UserId"),
            FullName = r.Field<string>("FullName"),
            MobileNumber = r.Field<string>("MobileNumber"),
            Email = r.Field<string>("Email"),
            RoleId = r.Field<int?>("RoleId"),
            RoleName = r.Field<string>("RoleName"),
            IsActive = r.Field<int>("IsActive"),
            GenderId = r.Field<int?>("GenderId"),

            LanguageId =
                r["LanguageId"] == DBNull.Value
                    ? new List<int>()
                    : r["LanguageId"]
                        .ToString()!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToList()
        }).ToList();
    }
    public UserDto? GetById(int id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@UserId", SqlDbType.Int) { Value = id } };
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetUserById", parameters);
        var row = dt.AsEnumerable().FirstOrDefault();
        if (row == null) return null;
        return new UserDto
        {
            UserId = row.Field<int>("UserId"),
            MobileNumber = row.Field<string>("MobileNumber"),
            Email = row.Field<string>("Email"),
            FullName = row.Field<string>("FullName"),
            RoleId = row.Field<int?>("RoleId"),
            RoleName = row.Field<string>("RoleName"),
            IsActive = row.Field<int>("IsActive"),
            GenderId = row.Field<int?>("GenderId"),
            LanguageId =
                row["LanguageId"] == DBNull.Value
                    ? new List<int>()
                    : row["LanguageId"]
                        .ToString()!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToList(),
            StateId = row.Field<int?>("StateId") ?? 0,
            DistrictIds =
                row["DistrictId"] == DBNull.Value
                    ? new List<int>()
                    : row["DistrictId"]
                        .ToString()!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToList(),
            BlockId = row.Field<int?>("BlockId") ?? 0,
        };
    }

    public int Create(UserDto user)
    {
        // ===== PASSWORD HASH =====
        object passwordHashValue;
        if (!string.IsNullOrWhiteSpace(user.Password))
            passwordHashValue = CommonController.EncryptSHAHash(user.Password);
        else
            passwordHashValue = (object?)user.PasswordHash ?? DBNull.Value;

        // ===== CREATED BY =====
        object createdByValue = DBNull.Value;
        try
        {
            var userClaim = _httpContextAccessor.HttpContext?
                .User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userClaim) &&
                int.TryParse(userClaim, out var createdBy))
            {
                createdByValue = createdBy;
            }
        }
        catch { }

        // ===== USER CREATE PARAMETERS =====
        var parameters = new SqlParameter[]
        {
        new("@Email", SqlDbType.NVarChar) { Value = (object?)user.Email ?? DBNull.Value },
        new("@Name", SqlDbType.NVarChar) { Value = (object?)user.FullName ?? DBNull.Value },
        new("@MobileNumber", SqlDbType.NVarChar) { Value = (object?)user.MobileNumber ?? DBNull.Value },
        new("@Password", SqlDbType.NVarChar) { Value = (object?)user.Password ?? DBNull.Value },
        new("@PasswordHash", SqlDbType.NVarChar) { Value = passwordHashValue },
        new("@RoleId", SqlDbType.Int) { Value = (object?)user.RoleId ?? DBNull.Value },
        new("@IsActive", SqlDbType.Bit) { Value = user.IsActive },
        new("@CreatedBy", SqlDbType.Int) { Value = createdByValue },
        new("@GenderId", SqlDbType.Int) { Value = (object?)user.GenderId ?? DBNull.Value },
        new("@StateId", SqlDbType.Int) { Value = (object?)user.StateId ?? DBNull.Value },
        new("@DistrictId", SqlDbType.NVarChar)
        {
           Value = user.DistrictIds != null && user.DistrictIds.Any()
           ? string.Join(",", user.DistrictIds)
           : DBNull.Value
        },
        new("@BlockId", SqlDbType.Int) { Value = (object?)user.BlockId ?? DBNull.Value },

        new("@LanguageId", SqlDbType.NVarChar)
        {
            Value = user.LanguageId != null && user.LanguageId.Any()
                ? string.Join(",", user.LanguageId)
                : DBNull.Value
        }
        };

        // ===== CREATE USER =====
        var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_CreateUser", parameters);


        int userid = Convert.ToInt32(dt.Rows[0]["UserId"]);

        return userid;
    }


    public bool Update(UserDto user)
    {
        // Compute password hash from provided Password when available (for update)
        object passwordHashValue;
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            passwordHashValue = CommonController.EncryptSHAHash(user.Password);
        }
        else
        {
            // if no new password provided, send existing PasswordHash (if present) or DBNull
            passwordHashValue = (object?)user.PasswordHash ?? DBNull.Value;
        }

        // Determine UpdatedBy from current principal (if available)
        object updatedByValue = DBNull.Value;
        try
        {
            var userClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(userClaim) && int.TryParse(userClaim, out var updatedBy))
            {
                updatedByValue = updatedBy;
            }
        }
        catch
        {
            updatedByValue = DBNull.Value;
        }

        var parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId },
            new SqlParameter("@MobileNumber", SqlDbType.NVarChar) { Value = (object?)user.MobileNumber ?? DBNull.Value },
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = (object?)user.Email ?? DBNull.Value },
            new SqlParameter("@Password", SqlDbType.NVarChar) { Value = (object?)user.Password ?? DBNull.Value },
            new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = passwordHashValue },
            new SqlParameter("@RoleId", SqlDbType.Int) { Value = (object?)user.RoleId ?? DBNull.Value },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive },
            new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedByValue },
            new SqlParameter("@Name", SqlDbType.NVarChar) { Value = (object?)user.FullName ?? DBNull.Value },
            new SqlParameter("@GenderId", SqlDbType.Int) { Value = (object?)user.GenderId ?? DBNull.Value },

            new SqlParameter("@StateId", SqlDbType.Int) { Value = (object?)user.StateId ?? DBNull.Value },
            new SqlParameter("@DistrictId", SqlDbType.NVarChar)
                {
                    Value = user.DistrictIds != null && user.DistrictIds.Any()
                        ? string.Join(",", user.DistrictIds)
                        : DBNull.Value
                },
        new SqlParameter("@BlockId", SqlDbType.Int) { Value = (object?)user.BlockId ?? DBNull.Value },
        new SqlParameter("@LanguageId", SqlDbType.NVarChar)
            {
                Value = user.LanguageId != null && user.LanguageId.Any()
                    ? string.Join(",", user.LanguageId)
                    : DBNull.Value
            }

                    };

        SqlUtils.ExecuteSP(Conn(), "dbo.sp_UpdateUser", parameters);

        return true;
    }
    public bool Delete(int id)
    {
        var parameters = new SqlParameter[] { new SqlParameter("@UserId", SqlDbType.Int) { Value = id } };
        SqlUtils.ExecuteSP(Conn(), "dbo.sp_DeleteUser", parameters);
        return true;
    }

    public async Task<List<UserDto>> SearchUser(string term)
    {
        return await Task.Run(() =>
        {
            var param = new SqlParameter[]
            {
            new SqlParameter("@Term", term)
            };

            // Call your stored procedure for searching users
            var dt = SqlUtils.ExecuteSP(Conn(), "dbo.sp_SearchUsers", param);

            return dt.AsEnumerable().Select(r => new UserDto
            {
                UserId = r.Field<int>("UserId"),
                FullName = r.Field<string>("FullName"),
                MobileNumber = r.Field<string>("MobileNumber"),
                Email = r.Field<string>("Email"),
                RoleId = r.Field<int?>("RoleId"),
                RoleName = r.Field<string>("RoleName"),
                IsActive = r.Field<int>("IsActive"),
                GenderId = r.Field<int?>("GenderId"),

                LanguageId =
                    r["LanguageId"] == DBNull.Value
                        ? new List<int>()
                        : r["LanguageId"]
                            .ToString()!
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                            .Where(x => x.HasValue)
                            .Select(x => x.Value)
                            .ToList()
            }).ToList();
        });
    }
}
