using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace Vikalp.Controllers.Api
{
    [ApiController]
    [Route("api/sync")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SyncDataController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SyncDataController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("save-asha-orientation")]
        public async Task<IActionResult> SaveAshaOrientation()
        {
            try
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new { message = "Invalid or expired token" });
                }

                // 🔹 Get UserId from JWT (same logic as download)
                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "uid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // 🔹 Read raw JSON body
                string rawJson;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return BadRequest(new { message = "Request payload is required" });
                }

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertAshaOrientationSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    message = "Sync completed successfully"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    message = "Database error during sync",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }
    }
}
