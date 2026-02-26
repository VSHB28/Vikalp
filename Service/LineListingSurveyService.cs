using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Vikalp.Utilities;

namespace Vikalp.Service
{
    public class LineListingSurveyService : ILineListingSurveyService
    {
        private readonly IConfiguration _config;

        public LineListingSurveyService(IConfiguration config)
        {
            _config = config;
        }

        private string Conn() => _config.GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("No connection string");

        // ===================== GET ALL =====================
      
        public async Task<(List<LineListingSurveyDto> Data, int TotalCount)> GetAllSurveys(
    int userId,
    int page,
    int pageSize,
    int? stateId,
    int? districtId,
    int? blockId,
    int? facilityId,
    int? subcentreId)
        {
            var parameters = new[]
            {
        new SqlParameter("@UserId", userId),
        new SqlParameter("@PageNumber", page),
        new SqlParameter("@PageSize", pageSize),
        new SqlParameter("@StateId", (object?)stateId ?? DBNull.Value),
        new SqlParameter("@DistrictId", (object?)districtId ?? DBNull.Value),
        new SqlParameter("@BlockId", (object?)blockId ?? DBNull.Value),
        new SqlParameter("@FacilityId", (object?)facilityId ?? DBNull.Value),
        new SqlParameter("@SubCenterId", (object)subcentreId ?? DBNull.Value)
    };

            var dt = await Task.Run(() =>
                SqlUtils.ExecuteSP(Conn(),
                    "dbo.sp_GetAllLineListingSurveysNew",
                    parameters));

            int totalRecords = 0;

            var list = dt.AsEnumerable().Select(r =>
            {
                totalRecords = Convert.ToInt32(r["TotalRecords"]);

                return new LineListingSurveyDto
                {
                    LineListId = r.Field<int>("LineListId"),
                    LineListGuid = r.Field<string>("LineListGuid"),

                    StateId = r.Field<int?>("StateId"),
                    DistrictId = r.Field<int?>("DistrictId"),
                    BlockId = r.Field<int?>("BlockId"),
                    VillageName = r.Field<string>("VillageName"),

                    FacilityId = r.Field<int?>("FacilityId"),
                    SubCenterId = r.Field<int?>("SubCenterId"),
                    ASHAId = r.Field<int?>("ASHAId"),
                    AnganwadiWorkerName = r.Field<string>("AnganwadiWorkerName"),

                    WomanName = r.Field<string>("WomanName"),
                    HusbandName = r.Field<string>("HusbandName"),
                    MobileNumber = r.Field<string>("MobileNumber"),

                    IsChildAvailable = r.Field<int?>("IsChildAvailable"),
                    ChildGender = r.Field<int?>("ChildGender"),
                    ChildDOB = r.Field<DateTime?>("ChildDOB"),
                    //MarriageDate = r.Field<DateTime?>("MarriageDate"),

                    IsCurrentlyPregnant = r.Field<int?>("IsCurrentlyPregnant"),
                    IsUsingFamilyPlanning = r.Field<int?>("IsUsingFamilyPlanning"),
                    FamilyPlanningMethod = r.Field<int?>("FamilyPlanningMethod"),

                    IsAwareOfAntara = r.Field<int?>("IsAwareOfAntara"),
                    SelectedMethodName = r.Field<string>("SelectedMethodName"),
                    ReasonForNonUsage = r.Field<string>("ReasonForNonUsage"),

                    IsConcent = r.Field<int?>("IsConcent"),
                    ConcentDate = r.Field<DateTime?>("ConcentDate"),
                    Signature = r.Field<string>("Signature"),

                    CreatedOn = r.Field<DateTime?>("CreatedOn"),
                    CreatedBy = r.Field<int?>("CreatedBy"),
                    UpdatedOn = r.Field<DateTime?>("UpdatedOn"),
                    UpdatedBy = r.Field<int?>("UpdatedBy")
                };
            }).ToList();

            return (list, totalRecords);
        }


        // ===================== GET BY ID =====================
        public LineListingSurveyDto? GetSurveyById(int id)
        {
            LineListingSurveyDto? dto = null;

            using (var conn = new SqlConnection(Conn()))
            using (var cmd = new SqlCommand("usp_GetLineListingSurveyById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dto = new LineListingSurveyDto
                        {
                            LineListId = reader.GetInt32(reader.GetOrdinal("LineListId")),
                            LineListGuid = reader["LineListGuid"] as string,
                            FacilityId = reader.IsDBNull(reader.GetOrdinal("FacilityId")) ? null : reader.GetInt32(reader.GetOrdinal("FacilityId")),
                            SubCenterId = reader.IsDBNull(reader.GetOrdinal("SubCenterId")) ? null : reader.GetInt32(reader.GetOrdinal("SubCenterId")),
                            WomanName = reader["Name"] as string,
                            MobileNumber = reader["MobileNumber"] as string,
                            IsConcent = reader.IsDBNull(reader.GetOrdinal("IsConcent")) ? null : reader.GetInt32(reader.GetOrdinal("IsConcent")),
                            CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            UpdatedOn = reader.IsDBNull(reader.GetOrdinal("UpdatedOn")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedBy"))
                        };
                    }
                }
            }

            return dto;
        }

        // ===================== INSERT =====================

        public async Task<bool> InsertLineListingAsync(LineListingSurveyCreateDto model, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

                using var cmd = new SqlCommand(
                    "dbo.sp_InsertLineListingSurvey_Json",
                    conn);

                cmd.CommandType = CommandType.StoredProcedure;

                // Convert full model to JSON
                string json = JsonSerializer.Serialize(model);

                cmd.Parameters.Add("@Json", SqlDbType.NVarChar).Value = json;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        // ===================== UPDATE =====================
        public async Task<LineListingSurveyDto> GetLineListingByIdAsync(int id)
        {
            LineListingSurveyDto survey = null;

            using (var conn = new SqlConnection(Conn()))
            using (var cmd = new SqlCommand("usp_GetLineListingById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    survey = new LineListingSurveyDto
                    {
                        Women = new List<LineListingWomanDto>()
                    };

                    while (await reader.ReadAsync())
                    {
                        // ================= HEADER (repeat ok) =================
                        survey.LineListId = Convert.ToInt32(reader["Id"]);
                        survey.StateId = reader["StateId"] != DBNull.Value ? Convert.ToInt32(reader["StateId"]) : 0;
                        survey.DistrictId = reader["DistrictId"] != DBNull.Value ? Convert.ToInt32(reader["DistrictId"]) : 0;
                        survey.BlockId = reader["BlockId"] != DBNull.Value ? Convert.ToInt32(reader["BlockId"]) : 0;
                        survey.VillageName = reader["VillageName"]?.ToString();


                        survey.FacilityId = reader["FacilityId"] != DBNull.Value
     ? Convert.ToInt32(reader["FacilityId"]) : null;

                        survey.SubCenterId = reader["SubCenterId"] != DBNull.Value
                            ? Convert.ToInt32(reader["SubCenterId"]) : null;

                        survey.ASHAId = reader["ASHAId"] != DBNull.Value
                            ? Convert.ToInt32(reader["ASHAId"]) : null;



                        survey.AnganwadiWorkerName = reader["AnganwadiWorkerName"]?.ToString();

                        survey.IsChildAvailable = reader["IsChildAvailable"] != DBNull.Value
                            ? Convert.ToInt32(reader["IsChildAvailable"])
                            : null;

                        survey.IsAwareOfAntara = reader["IsAwareOfAntara"] != DBNull.Value
                            ? Convert.ToInt32(reader["IsAwareOfAntara"])
                            : null;

                        survey.ReasonForNonUsage = reader["ReasonForNonUsage"]?.ToString();

                        // ================= WOMEN LIST =================
                        survey.Women.Add(new LineListingWomanDto
                        {
                            WomanName = reader["WomanName"]?.ToString(),
                            HusbandName = reader["HusbandName"]?.ToString(),
                            MobileNumber = reader["MobileNumber"]?.ToString(),
                            IsCurrentlyPregnant = reader["IsCurrentlyPregnant"] != DBNull.Value
    ? Convert.ToInt32(reader["IsCurrentlyPregnant"])
    : null,

                            IsUsingFamilyPlanning = reader["IsUsingFamilyPlanning"] != DBNull.Value
    ? Convert.ToInt32(reader["IsUsingFamilyPlanning"])
    : null



                        });
                    }
                }
            }

            return survey;
        }

        public async Task<bool> UpdateSurvey(LineListingSurveyUpdateDto model, int userId)
        {
            using var conn = new SqlConnection(Conn());
            using var cmd = new SqlCommand("dbo.sp_UpdateLineListingSurvey_Json", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            string json = JsonSerializer.Serialize(model);

            cmd.Parameters.Add("@Json", SqlDbType.NVarChar).Value = json;
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            Console.WriteLine("JSON being sent:");
            Console.WriteLine(json);

            await conn.OpenAsync();


            var result = await cmd.ExecuteScalarAsync();
            int rowsAffected = Convert.ToInt32(result);

            Console.WriteLine($"Rows affected: {rowsAffected}");

            return rowsAffected > 0;
        }


        // ===================== DELETE =====================
        public void DeleteSurvey(int id, int userId)
        {
            using var conn = new SqlConnection(Conn());
            using var cmd = new SqlCommand("usp_DeleteLineListingSurvey", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@LineListid", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", userId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public async Task<LineListingConsentDto?> GetConsentPrefillAsync(string guid)
        {
            using var conn = new SqlConnection(Conn());
            using var cmd = new SqlCommand(@"
        SELECT 
            LineListGuid,
            WomanName,
            HusbandName,
            MobileNumber,
            DistrictId,
            BlockId,
            SubCenterId,
            ASHAId,
            VillageName
        FROM tblLineListingSurvey
        WHERE LineListGuid = @Guid", conn);

            cmd.Parameters.AddWithValue("@Guid", guid);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new LineListingConsentDto
            {
                LineListGuid = reader["LineListGuid"].ToString()!,
                WomanName = reader["WomanName"].ToString()!,
                HusbandName = reader["HusbandName"] as string,
                MobileNumber = reader["MobileNumber"] as string,
                DistrictId = (int)reader["DistrictId"],
                BlockId = (int)reader["BlockId"],
                SubCenterId = (int)reader["SubCenterId"],
                ASHAId = (int)reader["ASHAId"],
                VillageName = reader["VillageName"] as string
            };
        }

        public async Task<bool> SaveConsentAsync(LineListingConsentDto model, int userId)
        {
            try
            {
                using var conn = new SqlConnection(Conn());
                using var cmd = new SqlCommand("sp_InsertMemberConsentFromLineListing", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LineListGuid", model.LineListGuid);
                cmd.Parameters.AddWithValue("@IsConsentGiven", model.IsConcent);
                cmd.Parameters.AddWithValue("@ConsentSignature",
                    (object?)model.Signature ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", userId);
                cmd.Parameters.AddWithValue("@ConsentDate", model.ConcentDate);
                cmd.Parameters.AddWithValue("@MobileHandledBy", model.MobileHandledBy);
                cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
