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

        [HttpPost("SaveUpdateLineList")]
        public async Task<IActionResult> SaveUpdateLineList()
        {
            try
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid or expired token"
                    });
                }

                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "uid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid user token"
                    });
                }

                string rawJson;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Request payload is required"
                    });
                }

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertLineListingSurveySync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Database error during sync",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("SaveUpdateHomeVisit")]
        public async Task<IActionResult> SaveUpdateHomeVisit()
        {
            try
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid or expired token"
                    });
                }

                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "uid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid user token"
                    });
                }

                string rawJson;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Request payload is required"
                    });
                }

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertHomeVisitSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Database error during sync",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("SaveUpdateLineListConcent")]
        public async Task<IActionResult> SaveUpdateLineListConcent()
        {
            try
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid or expired token"
                    });
                }

                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "uid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid user token"
                    });
                }

                string rawJson;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Request payload is required"
                    });
                }

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertMemberConsentDetailsSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Database error during sync",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("SaveUpdateHomeVisitFollowup")]
        public async Task<IActionResult> SaveUpdateHomeVisitFollowup()
        {
            try
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid or expired token"
                    });
                }

                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "uid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new
                    {
                        statusCode = 401,
                        message = "Invalid user token"
                    });
                }

                string rawJson;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawJson = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawJson))
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Request payload is required"
                    });
                }

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertHomeVisitFollowUpsSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Database error during sync",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Internal server error",
                    error = ex.Message
                });
            }            
        }
    }
}
