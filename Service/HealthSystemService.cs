using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Vikalp.Utilities;

namespace Vikalp.Service
{
    public class HealthSystemService : IHealthSystemService
    {
        private readonly IConfiguration _config;

        public HealthSystemService(IConfiguration config)
        {
            _config = config;
        }

        private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

        //    public async Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize, int? stateId,
        //int? districtId,
        //int? activityNameId)
        //    {
        //        var parameters = new[]
        //        {
        //    new SqlParameter("@UserId", userId),
        //    new SqlParameter("@PageNumber", pageNumber),
        //    new SqlParameter("@PageSize", pageSize),
        //    new SqlParameter("@StateId", DBNull.Value),
        //    new SqlParameter("@DistrictId", DBNull.Value),
        //    new SqlParameter("@ActivityId", DBNull.Value)
        //};

        //        var dt = await Task.Run(() =>
        //            SqlUtils.ExecuteSP(Conn(), "dbo.sp_getHealthSystemActivtyAllNew", parameters));

        //        int totalRecords = 0;

        //        var list = dt.AsEnumerable().Select(r =>
        //        {
        //            totalRecords = Convert.ToInt32(r["TotalRecords"]);

        //            return new HealthSystemActivityDto
        //            {
        //                ActivityId = Convert.ToInt32(r["ActivityId"]),
        //                ActivityName = r["ActivityName"]?.ToString(),
        //                TrainingVenue = r["TrainingVenue"]?.ToString(),
        //                NoOfParticipants = r["NoofParticipants"] != DBNull.Value
        //                                    ? Convert.ToInt32(r["NoofParticipants"])
        //                                    : 0,
        //                StartDate = r.Field<DateTime?>("StartDate"),
        //                EndDate = r.Field<DateTime?>("EndDate")
        //            };
        //        }).ToList();

        //        return (list, totalRecords);
        //    }
        public async Task<(List<HealthSystemActivityDto> Data, int TotalCount)> GetPagedAsync(int userId, int pageNumber, int pageSize, int? stateId, int? districtId, int? ActivityNameId)
        {
            var parameters = new[]
            {
        new SqlParameter("@UserId", userId),
        new SqlParameter("@PageNumber", pageNumber),
        new SqlParameter("@PageSize", pageSize),
        new SqlParameter("@StateId", stateId),
        new SqlParameter("@DistrictId", districtId),
        new SqlParameter("@ActivityNameId", ActivityNameId)
    };

            var dt = await Task.Run(() =>
                SqlUtils.ExecuteSP(Conn(), "dbo.sp_getHealthSystemActivtyAllNew", parameters));

            int totalRecords = 0;

            var list = dt.AsEnumerable().Select(r =>
            {
                totalRecords = Convert.ToInt32(r["TotalRecords"]);

                return new HealthSystemActivityDto
                {
                    ActivityId = Convert.ToInt32(r["ActivityId"]),
                    ActivityName = r["ActivityName"]?.ToString(),
                    TrainingVenue = r["TrainingVenue"]?.ToString(),
                    NoOfParticipants = r["NoofParticipants"] != DBNull.Value
                                        ? Convert.ToInt32(r["NoofParticipants"])
                                        : 0,
                    StartDate = r.Field<DateTime?>("StartDate"),
                    EndDate = r.Field<DateTime?>("EndDate")
                };
            }).ToList();

            return (list, totalRecords);
        }


        public async Task<bool> SaveHealthSystemActivityJsonAsync(HealthSystemActivityDto model, int userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Convert Activities list to CSV
                    string activitiesCsv =
                        model.Activities != null && model.Activities.Any()
                            ? string.Join(",", model.Activities)
                            : null;

                    var activityGuid = Guid.NewGuid().ToString();

                    var parameters = new SqlParameter[]
                    {
                new SqlParameter("@ActivityGuid", activityGuid),
                new SqlParameter("@ActivityNameId", model.ActivityNameId),
                new SqlParameter("@OtherActivity", (object?)model.OtherActivity ?? DBNull.Value),
                new SqlParameter("@StateId", (object?)model.StateId ?? DBNull.Value),
                new SqlParameter("@DistrictId", (object?)model.DistrictId ?? DBNull.Value),
                new SqlParameter("@TrainingVenue", (object?)model.TrainingVenue ?? DBNull.Value),
                new SqlParameter("@ActivityTypeId", (object?)model.ActivityTypeId ?? DBNull.Value),
                new SqlParameter("@Activities", (object?)activitiesCsv ?? DBNull.Value),
                new SqlParameter("@ActivityFormatId", (object?)model.ActivityFormatId ?? DBNull.Value),
                new SqlParameter("@StartDate", (object?)model.StartDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)model.EndDate ?? DBNull.Value),
                new SqlParameter("@Remarks", (object?)model.Remarks ?? DBNull.Value),
                new SqlParameter("@CreatedBy", userId)
                    };

                    SqlUtils.ExecuteSP(
                        Conn(),
                        "sp_InsertHealthSystemActivity",
                        parameters
                    );

                    return true;
                }
                catch (Exception ex)
                {
                    // TODO: log ex
                    return false;
                }
            });
        }

        public async Task<List<DropdownDto>> SearchFacilitiesAsync(string term)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Term", term)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_SearchFacilities", param);

                return dt.AsEnumerable().Select(r => new DropdownDto
                {
                    Id = r.Field<int>("FacilityId"),
                    Name = r.Field<string>("FacilityName")
                }).ToList();
            });
        }

        public async Task<List<ParticipantListDto>> GetparticipantByFacilityAsync(int facilityId)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@FacilityId", facilityId)
                };

                var dt = SqlUtils.ExecuteSP(
                    Conn(),
                    "sp_GetParticipantByFacility",
                    param
                );

                return dt.AsEnumerable()
                    .Select(r => new ParticipantListDto
                    {
                        FullName = r.Field<string>("FullName"),

                        FacilityId = r.Field<int>("FacilityId"),
                        // ? BIGINT ? string safely
                        Mobile = r["Mobile"] == DBNull.Value ? null : Convert.ToInt64(r["Mobile"]).ToString(),
                        FacilityName = r.Field<string?>("FacilityName")
                    })
                    .ToList();
            });
        }

        //added17-02-2026
        public async Task<HealthSystemActivityDto?> GetByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@ActivityId", id)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetHealthSystemActivityById", param);

                return dt.AsEnumerable().Select(r => new HealthSystemActivityDto
                {
                    ActivityId = r.Field<int>("ActivityId"),
                    ActivityNameId = r.Field<int?>("ActivityNameId"),
                    OtherActivity = r.Field<string?>("OtherActivity"),
                    StateId = r.Field<int?>("StateId"),
                    DistrictId = r.Field<int?>("DistrictId"),
                    TrainingVenue = r.Field<string?>("TrainingVenue"),
                    ActivityTypeId = r.Field<int?>("ActivityTypeId"),
                    ActivityFormatId = r.Field<int?>("ActivityFormatId"),


                    //NoOfParticipants = r.Field<int?>("NoofParticipants"),
                    StartDate = r.Field<DateTime?>("StartDate"),
                    EndDate = r.Field<DateTime?>("EndDate"),
                    Remarks = r.Field<string?>("Remarks"),
                    Activities = (r.Field<string?>("Activities") ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => int.Parse(x.Trim()))
        .ToList()
                }).FirstOrDefault();
            });
        }

        //this one 

        //public async Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync()
        //{
        //    var parameters = new SqlParameter[]
        //    {
        //new SqlParameter("@UserId", SqlDbType.Int) { Value = 1 }
        //    };

        //    // Run the synchronous DB call on thread-pool to avoid blocking
        //    var dt = await Task.Run(() => SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetHealthSystemParticipants", parameters));

        //    var list = dt.AsEnumerable().Select(r => new HealthSystemParticipantDto
        //    {
        //        ParticipantId = r.Field<int>("ParticipantId"),

        //        FullName = r.Field<string>("FullName"),
        //        ProviderTypeId = r.Field<int?>("ProviderTypeId"),
        //        FacilityId = r.Field<int?>("FacilityId"),
        //        FacilityName = r.Field<string?>("FacilityName"),
        //        FacilityTypeId = r.Field<int?>("FacilityTypeId"),
        //        FacilityTypeOther = r.Field<string?>("FacilityTypeOther"),
        //        InterventionFacility = r.Field<int?>("InterventionFacility"),
        //        DistrictId = r.Field<int?>("DistrictId"),
        //        GenderId = r.Field<int?>("GenderId"),
        //        Mobile = r.Field<string?>("Mobile"),
        //        VCATScorePreTest = r.Field<int?>("VCATScorePreTest"),
        //        VCATScorePostTest = r.Field<int?>("VCATScorePostTest"),
        //        RefresherTraining = r.Field<int?>("RefresherTraining"),
        //        Remarks = r.Field<string?>("Remarks"),
        //        CreatedAt = r.Field<DateTime?>("CreatedAt"),
        //        UpdatedAt = r.Field<DateTime?>("UpdatedAt")
        //    }).ToList();

        //    return list;
        //}



        public async Task<(List<HealthSystemParticipantDto> Data, int TotalCount)>
   GetAllParticipantAsync(
       int userId,
       int page,
       int pageSize,
       int? stateId,
       int? districtId,
       int? blockId,
       int? facilityId)
        {
            var parameters = new[]
            {
        new SqlParameter("@UserId", userId),
        new SqlParameter("@PageNumber", page),
        new SqlParameter("@PageSize", pageSize),
        new SqlParameter("@StateId", (object?)stateId ?? DBNull.Value),
        new SqlParameter("@DistrictId", (object?)districtId ?? DBNull.Value),
        new SqlParameter("@BlockId", (object?)blockId ?? DBNull.Value),
        new SqlParameter("@FacilityId", (object?)facilityId ?? DBNull.Value)
    };

            var dt = await Task.Run(() =>
                SqlUtils.ExecuteSP(Conn(),
                    "dbo.sp_GetHealthSystemParticipantsNew",
                    parameters));

            int totalRecords = 0;

            var list = dt.AsEnumerable().Select(r =>
            {
                totalRecords = Convert.ToInt32(r["TotalRecords"]);

                return new HealthSystemParticipantDto
                {
                    ParticipantId = r.Field<int>("ParticipantId"),
                    FullName = r.Field<string>("FullName"),
                    ProviderTypeId = r.Field<int?>("ProviderTypeId"),
                    FacilityId = r.Field<int?>("FacilityId"),
                    FacilityName = r.Field<string?>("FacilityName"),
                    FacilityTypeId = r.Field<int?>("FacilityTypeId"),
                    FacilityTypeOther = r.Field<string?>("FacilityTypeOther"),
                    InterventionFacility = r.Field<int?>("InterventionFacility"),
                    DistrictId = r.Field<int?>("DistrictId"),
                    GenderId = r.Field<int?>("GenderId"),
                    Mobile = r.Field<string?>("Mobile"),
                    VCATScorePreTest = r.Field<int?>("VCATScorePreTest"),
                    VCATScorePostTest = r.Field<int?>("VCATScorePostTest"),
                    RefresherTraining = r.Field<int?>("RefresherTraining"),
                    Remarks = r.Field<string?>("Remarks"),
                    CreatedAt = r.Field<DateTime?>("CreatedAt"),
                    UpdatedAt = r.Field<DateTime?>("UpdatedAt")
                };
            }).ToList();

            return (list, totalRecords);
        }










        //added17-02-2026ACTVITY UPDATE
        public async Task<bool> UpdateAsync(HealthSystemActivityDto model, int userId)
        {
            try
            {
                string activitiesCsv =
                    model.Activities != null && model.Activities.Any()
                        ? string.Join(",", model.Activities)
                        : null;

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@ActivityNameId", (object?)model.ActivityNameId ?? DBNull.Value),
            new SqlParameter("@OtherActivity", (object?)model.OtherActivity ?? DBNull.Value),
            new SqlParameter("@StateId", (object?)model.StateId ?? DBNull.Value),
            new SqlParameter("@DistrictId", (object?)model.DistrictId ?? DBNull.Value),
            new SqlParameter("@TrainingVenue", (object?)model.TrainingVenue ?? DBNull.Value),
            new SqlParameter("@ActivityTypeId", (object?)model.ActivityTypeId ?? DBNull.Value),
            new SqlParameter("@ActivityFormatId", (object?)model.ActivityFormatId ?? DBNull.Value),
            new SqlParameter("@StartDate", (object?)model.StartDate ?? DBNull.Value),
            new SqlParameter("@EndDate", (object?)model.EndDate ?? DBNull.Value),
            new SqlParameter("@Remarks", (object?)model.Remarks ?? DBNull.Value),
            new SqlParameter("@Activities", (object?)activitiesCsv ?? DBNull.Value),
            new SqlParameter("@UpdatedBy", userId),
            new SqlParameter("@ActivityId", model.ActivityId),


            };

                SqlUtils.ExecuteSP(
                    Conn(),
                    "sp_UpdateHealthSystemActivity",
                    parameters
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> SaveParticipantsAsync(DateTime dateOfActivity, int stateId, int? districtId, int? facilityTypeId, string? facilityTypeOther, List<HealthSystemParticipantDto> participants, int createdBy)
        {
            using SqlConnection con = new SqlConnection(Conn());
            await con.OpenAsync();

            foreach (var p in participants)
            {
                using SqlCommand cmd =
                    new SqlCommand("sp_InsertHealthSystemParticipant", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DateofActivity", dateOfActivity);
                cmd.Parameters.AddWithValue("@FullName", p.FullName);
                cmd.Parameters.AddWithValue("@StateId", stateId);
                cmd.Parameters.AddWithValue("@DistrictId", (object?)districtId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityId", (object?)p.FacilityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityName", (object?)p.FacilityName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityTypeId", (object?)facilityTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityTypeOther", (object?)facilityTypeOther ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InterventionFacility", (object?)p.InterventionFacility ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GenderId", (object?)p.GenderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", (object?)p.Mobile ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProviderTypeId", (object?)p.ProviderTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActivityNameId", (object?)p.ActivityNameId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VCATScorePreTest", (object?)p.VCATScorePreTest ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VCATScorePostTest", (object?)p.VCATScorePostTest ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefresherTraining", (object?)p.RefresherTraining ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", (object?)p.Remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        public Task<List<HealthSystemActivityDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId)
        {
            throw new NotImplementedException();
        }

        public HealthSystemParticipantSaveDto? GetParticipantsById(int id)
        {
            var param = new[]
            {
        new SqlParameter("@ParticipantId", id)
    };

            var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetParticipantsById", param);

            if (dt.Rows.Count == 0)
                return null;

            var header = new HealthSystemParticipantSaveDto
            {

                ActivityDate = dt.Rows[0].Field<DateTime>("ActivityDate"),
                StateId = dt.Rows[0].Field<int>("StateId"),
                DistrictId = dt.Rows[0].Field<int?>("DistrictId"),
                FacilityId = dt.Rows[0].Field<int?>("FacilityId"),

                ActivityTypeId = dt.Rows[0].Field<int?>("ActivityTypeId"),

                ActivityNameId = dt.Rows[0].Field<int>("ActivityNameId"),


                FacilityTypeId = dt.Rows[0].Field<int?>("FacilityTypeId"),
                FacilityTypeOther = dt.Rows[0].Field<string?>("FacilityTypeOther"),
                Participants = dt.AsEnumerable().Select(r => new HealthSystemParticipantDto
                {
                    ParticipantId = r.Field<int>("ParticipantId"),
                    FullName = r.Field<string>("FullName"),
                    FacilityId = r.Field<int?>("FacilityId"),
                    FacilityName = r.Field<string?>("FacilityName"),
                    //InterventionFacility = dt.Rows[0].Field<int?>("InterventionFacility"),
                    InterventionFacility = r.Field<int?>("InterventionFacility"),
                    Mobile = r.Field<string?>("Mobile"),
                    VCATScorePreTest = r.Field<int?>("VCATScorePreTest"),
                    VCATScorePostTest = r.Field<int?>("VCATScorePostTest"),
                    RefresherTraining = r.Field<int?>("RefresherTraining")
                }).ToList()
            };

            return header;
        }



        public async Task<bool> UpdateParticipantsAsync(DateTime dateofActivity, int stateId, int? districtId, int? facilityTypeId, int? activityTypeId, string? facilityTypeOther, int? activityNameId, int? providerTypeId, string? remarks, List<HealthSystemParticipantDto> participants, int userId)

        {
            using SqlConnection con = new SqlConnection(Conn());
            await con.OpenAsync();

            bool isUpdated = false;

            foreach (var p in participants)
            {
                using SqlCommand cmd = new SqlCommand("sp_UpdateHealthSystemParticipant", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ParticipantId", p.ParticipantId);
                //added this
                Console.WriteLine("ActivityTypeId: " + activityTypeId);

                cmd.Parameters.AddWithValue("@ActivityTypeId", (object?)activityTypeId ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@ActivityNameId", (object?)activityNameId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FullName", (object?)p.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProviderTypeId", (object?)providerTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StateId", stateId);
                cmd.Parameters.AddWithValue("@DistrictId", (object?)districtId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BlockId", (object?)p.BlockId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityTypeId", (object?)facilityTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityId", (object?)p.FacilityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityName", (object?)p.FacilityName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FacilityTypeOther", (object?)facilityTypeOther ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InterventionFacility", (object?)p.InterventionFacility ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActivityDate", dateofActivity);
                cmd.Parameters.AddWithValue("@GenderId", (object?)p.GenderId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", (object?)p.Mobile ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VCATScorePreTest", (object?)p.VCATScorePreTest ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VCATScorePostTest", (object?)p.VCATScorePostTest ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefresherTraining", (object?)p.RefresherTraining ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedBy", userId);


                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                    isUpdated = true;
            }

            return isUpdated;
        }



        public async Task<HealthSystemParticipantSaveDto?> GetParticipantsByIdAsync(int id)
        {
            var param = new[]
            {
        new SqlParameter("@ParticipantId", id)
    };

            var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetParticipantsById", param);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            var header = new HealthSystemParticipantSaveDto
            {
                ParticipantId = id,
                ActivityTypeId = dt.Rows[0].Field<int?>("ActivityTypeId"),
                ActivityDate = dt.Rows[0].Field<DateTime?>("ActivityDate"),
                StateId = dt.Rows[0].Field<int>("StateId"),
                DistrictId = dt.Rows[0].Field<int?>("DistrictId"),
                BlockId = dt.Rows[0].Field<int?>("BlockId"),
                FacilityId = dt.Rows[0].Field<int?>("FacilityId"),
                ProviderTypeId = dt.Rows[0].Field<int?>("ProviderTypeId"),
                ActivityNameId = (int)dt.Rows[0].Field<int?>("ActivityNameId"),
                Remarks = dt.Rows[0].Field<string?>("Remarks"),
                FacilityTypeId = dt.Rows[0].Field<int?>("FacilityTypeId"),
                FacilityTypeOther = dt.Rows[0].Field<string?>("FacilityTypeOther"),
                Participants = dt.AsEnumerable().Select(r => new HealthSystemParticipantDto
                {
                    ParticipantId = r.Field<int>("ParticipantId"),
                    FullName = r.Field<string>("FullName"),
                    FacilityId = r.Field<int?>("FacilityId"),
                    FacilityName = r.Field<string?>("FacilityName"),
                    Mobile = r.Field<string?>("Mobile"),
                    VCATScorePreTest = r.Field<int?>("VCATScorePreTest"),
                    VCATScorePostTest = r.Field<int?>("VCATScorePostTest"),
                    RefresherTraining = r.Field<int?>("RefresherTraining")
                }).ToList()
            };

            return header;
        }

    }
}
