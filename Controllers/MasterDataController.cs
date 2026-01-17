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
    [HttpPost("download")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Download()
    {
        if (!(User?.Identity?.IsAuthenticated ?? false))
        {
            return Unauthorized(new { message = "Invalid or expired token" });
        }

        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "uid" || c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");

        var roleClaim = User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.Role || c.Type == "RoleId");

        int? userIdInt = null;
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedId))
        {
            userIdInt = parsedId;
        }

        int? roleIdInt = null;
        if (roleClaim != null && int.TryParse(roleClaim.Value, out var parsedRole))
        {
            roleIdInt = parsedRole;
        }

        var connStr = _config.GetConnectionString("DefaultConnection");
        string jsonResult;

        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.sp_GetMasterData", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)
            {
                Value = userIdInt.HasValue ? (object)userIdInt.Value : DBNull.Value
            });
            cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int)
            {
                Value = roleIdInt.HasValue ? (object)roleIdInt.Value : DBNull.Value
            });

            conn.Open();
            jsonResult = (string)cmd.ExecuteScalar(); // Stored proc returns JSON string
        }

        return Content(jsonResult, "application/json");
    }
}