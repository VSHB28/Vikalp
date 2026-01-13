using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Vikalp.Utilities;

namespace Vikalp.Controllers.Api;

[ApiController]
[Route("api/master")]
public class MasterDataController : ControllerBase
{
    private readonly IConfiguration _config;

    public MasterDataController(IConfiguration config) => _config = config;

    // Require JWT Bearer for mobile clients
    [HttpGet("download")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Download()
    {
        // Quick header check
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return Unauthorized(new { message = "Authorization header missing" });
        }

        // Extract raw token (supports "Bearer <token>" and raw token)
        var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader.Substring("Bearer ".Length).Trim()
            : authHeader.Trim();

        // Ensure authentication middleware validated the token
        if (!(User?.Identity?.IsAuthenticated ?? false))
        {
            return Unauthorized(new { message = "Invalid or expired token" });
        }

        // Read claims from the validated principal (preferred)
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "RoleId");

        int? userIdInt = null;

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedId))
        {
            userIdInt = parsedId;
        }

        // Try to get numeric role id if available in claims
        int? roleIdInt = null;
        if (roleClaim != null && int.TryParse(roleClaim.Value, out var parsedRole))
        {
            roleIdInt = parsedRole;
        }

        // As a fallback, parse the raw JWT payload (does NOT validate signature — middleware already validated),
        // this lets you read claim names that might not map to ClaimTypes above.
        JwtSecurityToken? jwt = null;
        try
        {
            jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            // ignore parse errors - the middleware already ensured the token is valid for the request
        }

        if (jwt != null && !userIdInt.HasValue)
        {
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "uid");
            if (sub != null && int.TryParse(sub.Value, out var subInt))
            {
                userIdInt = subInt;
            }
        }

        // Check token expiry if parsed (optional — middleware handles this too)
        if (jwt != null && jwt.ValidTo < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Token expired" });
        }

        // At this point you have:
        // - userIdGuid (nullable Guid) extracted from claims
        // - roleClaim (Claim) or roleIdInt (nullable int) if available
        // Use those to scope master data

        var conn = _config.GetConnectionString("DefaultConnection");

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", SqlDbType.Int)
{
    Value = userIdInt.HasValue ? (object)userIdInt.Value : DBNull.Value
},
            // Keep RoleId as string if your stored proc expects NVARCHAR, otherwise adapt the type
            new SqlParameter("@RoleId", SqlDbType.NVarChar)
            {
                Value = roleClaim?.Value ?? (object)DBNull.Value
            }
        };

        var dt = SqlUtils.ExecuteSP(conn, "dbo.sp_GetMasterData", parameters);

        var json = DataTableToJson(dt);
        return Content(json, "application/json");
    }

    private static string DataTableToJson(DataTable dt)
    {
        var list = new List<Dictionary<string, object?>>();
        foreach (DataRow r in dt.Rows)
        {
            var dict = new Dictionary<string, object?>();
            foreach (DataColumn c in dt.Columns)
            {
                dict[c.ColumnName] = r[c] == DBNull.Value ? null : r[c];
            }
            list.Add(dict);
        }
        return JsonSerializer.Serialize(list);
    }
}