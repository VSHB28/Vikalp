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

                Log(userId, rawJson, "save-asha-orientation");

                var connStr = _config.GetConnectionString("DefaultConnection");

                var results = new List<object>();

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertAshaOrientationSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                RecordType = reader["RecordType"].ToString(),
                                RecordGuid = reader["RecordGuid"].ToString(),
                                ActionType = reader["ActionType"].ToString(),
                                Message = reader["Message"].ToString()
                            });
                        }
                    }
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync processed",
                    results = results
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

                Log(userId, rawJson, "SaveUpdateLineList");

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertLineListingSurveySync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    var results = new List<object>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                lineListGuid = reader["LineListGuid"]?.ToString(),
                                actionTaken = reader["ActionTaken"]?.ToString(),
                                statusMsg = reader["StatusMsg"]?.ToString()
                            });
                        }
                    }

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Sync completed successfully",
                        results
                    });
                }
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

                Log(userId, rawJson, "SaveUpdateHomeVisit");

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertHomeVisitSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    var results = new List<object>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                visitGuid = reader["VisitGuid"]?.ToString(),
                                actionTaken = reader["ActionTaken"]?.ToString(),
                                statusMsg = reader["StatusMsg"]?.ToString()
                            });
                        }
                    }

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Sync completed successfully",
                        results
                    });
                }
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

                Log(userId, rawJson, "SaveUpdateLineListConcent");

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertMemberConsentDetailsSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    var results = new List<object>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                consentGuid = reader["ConsentGuid"]?.ToString(),
                                actionTaken = reader["ActionTaken"]?.ToString(),
                                statusMsg = reader["StatusMsg"]?.ToString()
                            });
                        }
                    }

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Sync completed successfully",
                        results
                    });
                }
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

                Log(userId, rawJson, "SaveUpdateHomeVisitFollowup");

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertHomeVisitFollowUpsSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    var result = new List<object>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new
                            {
                                FollowupGuId = reader["FollowupGuId"].ToString(),
                                StatusCode = Convert.ToInt32(reader["StatusCode"]),
                                Message = reader["Message"].ToString()
                            });
                        }
                    }
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Sync processed",
                        results = result
                    });
                }
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

        [HttpPost("SaveUpdateFacilityProfile")]
        public async Task<IActionResult> SaveUpdateFacilityProfile()
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

                Log(userId, rawJson, "SaveUpdateFacilityProfile");

                var connStr = _config.GetConnectionString("DefaultConnection");

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertFacilityProfileSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    var results = new List<object>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                profileGuid = reader["profileGuid"]?.ToString(),
                                actionTaken = reader["actionTaken"]?.ToString(),
                                statusMsg = reader["statusMsg"]?.ToString()
                            });
                        }
                    }
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Sync completed successfully",
                        results = results
                    });
                }
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

        [HttpPost("SaveUpdateHrStatus")]
        public async Task<IActionResult> SaveUpdateHrStatus()
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

                Log(userId, rawJson, "SaveUpdateHrStatus");

                var connStr = _config.GetConnectionString("DefaultConnection");
                var results = new List<object>();

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertHRstatusSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new
                            {
                                HrGuid = reader["HrGuid"].ToString(),
                                ActionTaken = reader["ActionTaken"].ToString(),
                                StatusMsg = reader["StatusMsg"].ToString()
                            });
                        }
                    }
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully",
                    results
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

        [HttpPost("SaveUpdateEventActivity")]
        public async Task<IActionResult> SaveUpdateEventActivity()
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

                Log(userId, rawJson, "SaveUpdateEventActivity");

                var connStr = _config.GetConnectionString("DefaultConnection");
                var results = new List<object>();

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.sp_InsertEventActivitiesSync", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = rawJson;

                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // Debug: see what columns are coming back
                        // for (int i = 0; i < reader.FieldCount; i++)
                        //     Console.WriteLine(reader.GetName(i));

                        while (await reader.ReadAsync())
                        {
                            results.Add(new
                            {
                                EventGuid = reader["EventGuid"].ToString(),
                                ActionTaken = reader["ActionTaken"].ToString(),
                                StatusMsg = reader["StatusMsg"].ToString()
                            });
                        }
                    }
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sync completed successfully",
                    results
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

        //============================= = LOGGING METHOD =============================
        public void Log(int userId, string jsonPayload, string api)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("dbo.sp_LogSyncRequest", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@JsonPayload", jsonPayload);
                cmd.Parameters.AddWithValue("@API", api);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
