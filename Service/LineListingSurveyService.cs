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
                    ChildCount = r.Field<int?>("ChildCount"),
                    ChildGender = r.Field<int?>("ChildGender"),
                    ChildAgeMonth = r.Field<int?>("ChildAgeMonth"),
                    ChildAgeYear = r.Field<int?>("ChildAgeYear"),
                    //MarriageDate = r.Field<DateTime?>("MarriageDate"),

                    IsCurrentlyPregnant = r.Field<int?>("IsCurrentlyPregnant"),
                    IsUsingFamilyPlanning = r.Field<int?>("IsUsingFamilyPlanning"),
                    FamilyPlanningMethod = r.Field<int?>("FamilyPlanningMethod"),

                    IsAwareOfAntara = r.Field<int?>("IsAwareOfAntara"),
                    ChoosenMethodToday = r.Field<int?>("ChoosenMethodToday"),
                    SelectedMethodName = r.Field<int?>("SelectedMethodName"),
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
            try
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
                        if (await reader.ReadAsync())
                        {
                            survey = new LineListingSurveyDto
                            {
                                LineListId = Convert.ToInt32(reader["Id"]),
                                StateId = reader["StateId"] != DBNull.Value ? Convert.ToInt32(reader["StateId"]) : null,
                                DistrictId = reader["DistrictId"] != DBNull.Value ? Convert.ToInt32(reader["DistrictId"]) : null,
                                BlockId = reader["BlockId"] != DBNull.Value ? Convert.ToInt32(reader["BlockId"]) : null,
                                VillageName = reader["VillageName"]?.ToString(),

                                FacilityId = reader["FacilityId"] != DBNull.Value ? Convert.ToInt32(reader["FacilityId"]) : null,
                                SubCenterId = reader["SubCenterId"] != DBNull.Value ? Convert.ToInt32(reader["SubCenterId"]) : null,
                                ASHAId = reader["ASHAId"] != DBNull.Value ? Convert.ToInt32(reader["ASHAId"]) : null,
                                AnganwadiWorkerName = reader["AnganwadiWorkerName"]?.ToString(),

                                WomanName = reader["WomanName"]?.ToString(),
                                HusbandName = reader["HusbandName"]?.ToString(),
                                MobileNumber = reader["MobileNumber"]?.ToString(),

                                // ✅ ADDED MISSING FIELDS
                                MarriageMonth = reader["MarriageMonth"] != DBNull.Value ? Convert.ToInt32(reader["MarriageMonth"]) : null,
                                MarriageYear = reader["MarriageYear"] != DBNull.Value ? Convert.ToInt32(reader["MarriageYear"]) : null,

                                IsChildAvailable = reader["IsChildAvailable"] != DBNull.Value ? Convert.ToInt32(reader["IsChildAvailable"]) : null,
                                ChildCount = reader["ChildCount"] != DBNull.Value ? Convert.ToInt32(reader["ChildCount"]) : null,
                                ChildGender = reader["ChildGender"] != DBNull.Value ? Convert.ToInt32(reader["ChildGender"]) : null,
                                ChildAgeMonth = reader["ChildAgeMonth"] != DBNull.Value ? Convert.ToInt32(reader["ChildAgeMonth"]) : null,
                                ChildAgeYear = reader["ChildAgeYear"] != DBNull.Value ? Convert.ToInt32(reader["ChildAgeYear"]) : null,

                                IsCurrentlyPregnant = reader["IsCurrentlyPregnant"] != DBNull.Value ? Convert.ToInt32(reader["IsCurrentlyPregnant"]) : null,
                                IsUsingFamilyPlanning = reader["IsUsingFamilyPlanning"] != DBNull.Value ? Convert.ToInt32(reader["IsUsingFamilyPlanning"]) : null,
                                FamilyPlanningMethod = reader["FamilyPlanningMethod"] != DBNull.Value ? Convert.ToInt32(reader["FamilyPlanningMethod"]) : null,
                                IsAwareOfAntara = reader["IsAwareOfAntara"] != DBNull.Value ? Convert.ToInt32(reader["IsAwareOfAntara"]) : null,
                                ChoosenMethodToday = reader["ChoosenMethodToday"] != DBNull.Value ? Convert.ToInt32(reader["ChoosenMethodToday"]) : null,
                                SelectedMethodName = reader["SelectedMethodName"] != DBNull.Value ? Convert.ToInt32(reader["SelectedMethodName"]) : null,
                                ReasonForNonUsage = reader["ReasonForNonUsage"]?.ToString(),

                                IsConcent = reader["IsConcent"] != DBNull.Value ? Convert.ToInt32(reader["IsConcent"]) : null,
                                ConcentDate = reader["ConcentDate"] != DBNull.Value ? Convert.ToDateTime(reader["ConcentDate"]) : null,
                                Signature = reader["Signature"]?.ToString(),

                                CreatedOn = reader["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedOn"]) : null,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : null,
                                UpdatedOn = reader["UpdatedOn"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedOn"]) : null,
                                UpdatedBy = reader["UpdatedBy"] != DBNull.Value ? Convert.ToInt32(reader["UpdatedBy"]) : null,
                            };
                        }
                    }
                }

                return survey;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<bool> UpdateSurvey(LineListingSurveyUpdateDto model, int userId)
        {
            try
            {
                using var conn = new SqlConnection(Conn());
                using var cmd = new SqlCommand("dbo.sp_UpdateLineListingSurvey_Json", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                string json = JsonSerializer.Serialize(model);

                Console.WriteLine("=== JSON TO SP ===");
                Console.WriteLine(json);

                cmd.Parameters.Add("@Json", SqlDbType.NVarChar).Value = json;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();

                Console.WriteLine("=== SP RESULT: " + result);
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== UPDATE ERROR: " + ex.Message);
                Console.WriteLine("=== INNER: " + ex.InnerException?.Message);
                throw;
            }
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
                cmd.Parameters.AddWithValue("@IsConsetHardCopy", model.IsCollectedHardCopy);
                cmd.Parameters.AddWithValue("@CreatedBy", userId);
                cmd.Parameters.AddWithValue("@MobileHandledBy", model.MobileHandledBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ConsentDate", model.ConsentDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber ?? (object)DBNull.Value);

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
