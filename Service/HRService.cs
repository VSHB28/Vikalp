using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

namespace Vikalp.Service
{
    public class HRService : IHRService
    {
        private readonly IConfiguration _config;
        private string _connectionString;

        public HRService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Conn()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }


        public async Task SaveHrStatusAsync(HrStatusDto model)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                parameters.Add("@HrId", model.HrId);
                parameters.Add("@Name", model.Name);
                parameters.Add("@DesignationId", model.DesignationId);
                parameters.Add("@GenderId", model.GenderId);
                parameters.Add("@Mobile", model.Mobile);
                parameters.Add("@FacilityTypeId", model.FacilityTypeId);
                parameters.Add("@FacilityId", model.FacilityId);
                parameters.Add("@TrainedAntaraGovt", model.TrainedAntaraGovt);
                parameters.Add("@TrainedAntaraIDF", model.TrainedAntaraIDF);
                parameters.Add("@AttendentVCAT", model.AttendentVCAT);
                parameters.Add("@TrainedInIUCD", model.TrainedInIUCD);
                parameters.Add("@TrainedInFPLMIS", model.TrainedInFPLMIS);
                parameters.Add("@TrainedInCACS_MMA", model.TrainedInCACS_MMA);
                parameters.Add("@UserId", model.CreatedBy);

                await con.ExecuteAsync(
                    "usp_SaveHrStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<HrStatusDto> GetHrStatusAsync(int hrId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<HrStatusDto>(
                    "usp_GetFacilityHrstatus",
                    new { HrId = hrId },
                    commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<List<HrStatusDto>> GetHrListAsync()
        {
            using var con = Conn();

            var result = await con.QueryAsync<HrStatusDto>(
                "SELECT HrId, Name, Mobile, DesignationId, GenderId, FacilityId,AttendentVCAT FROM tblHRstatus");

            return result.ToList();
        }




        public async Task<(List<HrStatusDto> HrList, int TotalCount)> GetHrListPagedAsync(int pageNumber, int pageSize)
        {
            using var con = Conn();

            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (pageNumber - 1) * pageSize);
            parameters.Add("@Fetch", pageSize);

            // Stored procedure approach (recommended)
            var hrList = await con.QueryAsync<HrStatusDto>(
                "usp_GetHrListPaged", // create this in SQL
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Total count
            var totalCount = await con.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblHRstatus");

            return (hrList.ToList(), totalCount);
        }

    }
}