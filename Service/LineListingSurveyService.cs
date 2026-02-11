using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

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
        public IEnumerable<LineListingSurveyDto> GetAllSurveys(int userId)
        {
            var list = new List<LineListingSurveyDto>();

            using var conn = new SqlConnection(Conn());
            using var cmd = new SqlCommand("sp_GetAllLineListingSurveys", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // ✅ Add the @UserId parameter
            cmd.Parameters.AddWithValue("@UserId", userId);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapReaderToDto(reader));
            }

            return list;
        }
        private LineListingSurveyDto MapReaderToDto(SqlDataReader reader)
        {
            return new LineListingSurveyDto
            {
                LineListId = reader.GetInt32(reader.GetOrdinal("LineListId")),
                LineListGuid = reader["LineListGuid"].ToString(),

                StateId = reader["StateId"] as int?,
                DistrictId = reader["DistrictId"] as int?,
                BlockId = reader["BlockId"] as int?,
                VillageName = reader["VillageName"] as string,

                FacilityId = reader["FacilityId"] as int?,
                SubCenterId = reader["SubCenterId"] as int?,
                ASHAId = reader["ASHAId"] as int?,
                AnganwadiWorkerName = reader["AnganwadiWorkerName"] as string,

                WomanName = reader["WomanName"] as string,
                HusbandName = reader["HusbandName"] as string,
                MobileNumber = reader["MobileNumber"] as string,

                IsChildAvailable = reader["IsChildAvailable"] as int?,
                ChildGender = reader["ChildGender"] as int?,
                ChildDOB = reader["ChildDOB"] as DateTime?,
                MarriageDate = reader["MarriageDate"] as DateTime?,

                IsCurrentlyPregnant = reader["IsCurrentlyPregnant"] as int?,
                IsUsingFamilyPlanning = reader["IsUsingFamilyPlanning"] as int?,

                FamilyPlanningMethod = reader["FamilyPlanningMethod"] != DBNull.Value
                    ? reader["FamilyPlanningMethod"].ToString()!
                        .Split(',')
                        .Select(int.Parse)
                        .ToList()
                    : new List<int>(),

                IsAwareOfAntara = reader["IsAwareOfAntara"] as int?,
                SelectedMethodName = reader["SelectedMethodName"] as string,
                ReasonForNonUsage = reader["ReasonForNonUsage"] as string,

                IsConcent = reader["IsConcent"] as int?,
                ConcentDate = reader["ConcentDate"] as DateTime?,
                Signature = reader["Signature"] as string,

                CreatedOn = reader["CreatedOn"] as DateTime?,
                CreatedBy = reader["CreatedBy"] as int?,
                UpdatedOn = reader["UpdatedOn"] as DateTime?,
                UpdatedBy = reader["UpdatedBy"] as int?
            };
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

        // ===================== UPDATE =====================
        public void UpdateSurvey(LineListingSurveyDto survey)
        {
            using var conn = new SqlConnection(Conn());
            using var cmd = new SqlCommand("usp_UpdateLineListingSurvey", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@LineListGuid", survey.LineListGuid);

            cmd.Parameters.AddWithValue("@WomanName", survey.WomanName);
            cmd.Parameters.AddWithValue("@HusbandName", (object?)survey.HusbandName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MobileNumber", (object?)survey.MobileNumber ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsChildAvailable", (object?)survey.IsChildAvailable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChildGender", (object?)survey.ChildGender ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChildDOB", (object?)survey.ChildDOB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MarriageDate", (object?)survey.MarriageDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsCurrentlyPregnant", (object?)survey.IsCurrentlyPregnant ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsUsingFamilyPlanning", (object?)survey.IsUsingFamilyPlanning ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@FamilyPlanningMethod",
                survey.FamilyPlanningMethod != null
                    ? string.Join(",", survey.FamilyPlanningMethod)
                    : DBNull.Value);

            cmd.Parameters.AddWithValue("@IsAwareOfAntara", (object?)survey.IsAwareOfAntara ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SelectedMethodName", (object?)survey.SelectedMethodName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReasonForNonUsage", (object?)survey.ReasonForNonUsage ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsConcent", (object?)survey.IsConcent ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ConcentDate", (object?)survey.ConcentDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Signature", (object?)survey.Signature ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@UpdatedBy", survey.UpdatedBy);

            conn.Open();
            cmd.ExecuteNonQuery();
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
