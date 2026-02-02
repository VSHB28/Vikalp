using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Vikalp.Models;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
using Vikalp.Utilities;

namespace Vikalp.Service
{
    public class FacilityProfileService : IFacilityProfileService
    {
        private readonly IConfiguration _config;

        public FacilityProfileService(IConfiguration config)
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
    }
}
