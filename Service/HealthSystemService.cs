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

        public async Task<List<HealthSystemActivityDto>> GetAllAsync()
        {
            var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = 1 },
                };
            // Run the synchronous DB call on thread-pool to avoid blocking
            var dt = await Task.Run(() => SqlUtils.ExecuteSP(Conn(), "dbo.sp_getHealthSystemActivtyAll", parameters));

            var list = dt.AsEnumerable().Select(r => new HealthSystemActivityDto
            {
                ActivityName = r.Field<string>("ActivityName"),
                OtherActivity = r.Field<string?>("OtherActivity"),
                StateId = r.Field<int?>("StateId"),
                DistrictId = r.Field<int?>("DistrictId"),
                TrainingVenue = r.Field<string?>("TrainingVenue"),
                ActivityTypeId = r.Field<int?>("ActivityTypeId"),
                ActivityFormatId = r.Field<int?>("ActivityFormatId"),
                NoOfParticipants = r.Field<int?>("NoofParticipants"),
                StartDate = r.Field<DateTime?>("StartDate"),
                EndDate = r.Field<DateTime?>("EndDate"),
                Remarks = r.Field<string?>("Remarks")
            }).ToList();

            return list;
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
                new SqlParameter("@NoOfParticipants", (object?)model.NoOfParticipants ?? DBNull.Value),
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

        public async Task<MstFacilityDto?> GetFacilityByIdAsync(int facilityId)
        {
            return await Task.Run(() =>
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@FacilityId", facilityId)
                };

                var dt = SqlUtils.ExecuteSP(Conn(), "sp_GetFacilityById", param);

                return dt.AsEnumerable().Select(r => new MstFacilityDto
                {
                    FacilityId = r.Field<int>("FacilityId"),
                    FacilityName = r.Field<string>("FacilityName"),
                    NinNumber = r.Field<long?>("NinNumber")
                }).FirstOrDefault();
            });
        }
        public async Task<List<HealthSystemParticipantDto>> GetAllParticipantAsync()
        {
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@UserId", SqlDbType.Int) { Value = 1 }
            };

            // Run the synchronous DB call on thread-pool to avoid blocking
            var dt = await Task.Run(() => SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetHealthSystemParticipants", parameters));

            var list = dt.AsEnumerable().Select(r => new HealthSystemParticipantDto
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
            }).ToList();

            return list;
        }

        public async Task<bool> SaveHealthSystemParticipantJsonAsync(HealthSystemParticipantDto model, int userId)
        {
            return await Task.Run(() =>
            {
                try
                {

                    var activityGuid = Guid.NewGuid().ToString();

                    var parameters = new SqlParameter[]
                    {
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
    }
}
