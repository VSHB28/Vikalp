using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Utilities;

namespace Vikalp.Service
{
    public class HealthSystemService
    {
        private readonly IConfiguration _config;

        public HealthSystemService(IConfiguration config)
        {
            _config = config;
        }

        private string Conn() => _config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string");

        public async Task<List<HealthSystemActivityDto>> GetAllAsync()
        {
            // Run the synchronous DB call on thread-pool to avoid blocking
            var dt = await Task.Run(() => SqlUtils.ExecuteSP(Conn(), "dbo.sp_GetVenueOrientationSummary", null));

            var list = dt.AsEnumerable().Select(r => new HealthSystemActivityDto
            {
               
            }).ToList();

            return list;
        }
    }
}
