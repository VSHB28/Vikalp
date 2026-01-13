using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Vikalp.Utilities;
using Vikalp.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Vikalp.Service;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthResult { Success = false, Message = "Username and password required" };
        }

        // Execute sync SP on threadpool to avoid blocking
        var dt = await Task.Run(() =>
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username.Trim() },
                new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password.Trim() }
            };
            return SqlUtils.ExecuteSP(Conn(), "dbo.sp_LoginUser", parameters);
        });

        if (dt == null || dt.Rows.Count == 0)
            return new AuthResult { Success = false, Message = "Invalid username or password" };

        var row = dt.Rows[0];

        // Try read UserId (guid) if available
        Guid userId = Guid.Empty;
        if (dt.Columns.Contains("UserId") && row["UserId"] != DBNull.Value)
        {
            if (row["UserId"] is Guid g) userId = g;
            else Guid.TryParse(Convert.ToString(row["UserId"]), out userId);
        }

        // RoleId may be int or string; parse to int (fallback 0)
        int roleInt = 0;
        if (dt.Columns.Contains("RoleId") && row["RoleId"] != DBNull.Value)
        {
            var roleRaw = row["RoleId"].ToString() ?? string.Empty;
            int.TryParse(roleRaw, out roleInt);
        }

        var returnedUsername = dt.Columns.Contains("Username") && row["Username"] != DBNull.Value
            ? row["Username"].ToString() ?? username
            : username;

        return new AuthResult
        {
            Success = true,
            UserId = userId,
            RoleId = roleInt,
            Username = returnedUsername
        };
    }
}