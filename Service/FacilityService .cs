using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

namespace Vikalp.Service.Implementations
{
    public class FacilityService : IFacilityService
    {
        private readonly string _connectionString;

        public FacilityService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<FacilityDto>> GetByBlockAsync(int blockId)
        {
            var list = new List<FacilityDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Facility_CRUD", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "GetByBlock";
            cmd.Parameters.Add("@BlockId", SqlDbType.Int).Value = blockId;

            await con.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(MapFacility(dr));
            }

            return list;
        }

        public async Task<FacilityDto?> GetByIdAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Facility_CRUD", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "GetById";
            cmd.Parameters.Add("@FacilityId", SqlDbType.Int).Value = id;

            await con.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
            {
                return MapFacility(dr);
            }

            return null;
        }


        public async Task AddAsync(FacilityDto dto)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("USP_Facility_CRUD", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Insert";
                cmd.Parameters.Add("@FacilityName", SqlDbType.NVarChar).Value = dto.FacilityName ?? "";

                // 🔑 THIS WAS MISSING
                cmd.Parameters.Add("@FacilityType", SqlDbType.Int)
                              .Value = dto.FacilityType;

                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = dto.IsActive;
                cmd.Parameters.Add("@NinNumber", SqlDbType.NVarChar)
                              .Value = (object?)dto.NinNumber ?? DBNull.Value;
                cmd.Parameters.Add("@BlockId", SqlDbType.Int).Value = dto.BlockId;
                cmd.Parameters.Add("@IsIntervention", SqlDbType.Bit).Value = dto.IsIntervention;

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("FacilityService.AddAsync failed: " + ex.Message, ex);
            }
        }


        public async Task UpdateAsync(FacilityDto dto)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Facility_CRUD", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Update";
            cmd.Parameters.Add("@FacilityId", SqlDbType.Int).Value = dto.FacilityId;
            cmd.Parameters.Add("@FacilityName", SqlDbType.NVarChar).Value = dto.FacilityName;
            //cmd.Parameters.Add("@FacilityType", SqlDbType.NVarChar).Value = dto.FacilityType;
            cmd.Parameters.Add("@FacilityType", SqlDbType.Int).Value = dto.FacilityType;

            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = dto.IsActive;
            cmd.Parameters.Add("@NinNumber", SqlDbType.NVarChar).Value = dto.NinNumber ?? (object)DBNull.Value;
            cmd.Parameters.Add("@BlockId", SqlDbType.Int).Value = dto.BlockId;
            cmd.Parameters.Add("@IsIntervention", SqlDbType.Bit).Value = dto.IsIntervention;

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Facility_CRUD", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Delete";
            cmd.Parameters.Add("@FacilityId", SqlDbType.Int).Value = id;

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private static FacilityDto MapFacility(SqlDataReader dr)
        {
            return new FacilityDto
            {
                FacilityId = dr["FacilityId"] != DBNull.Value
                                ? Convert.ToInt32(dr["FacilityId"])
                                : 0,

                FacilityName = dr["FacilityName"]?.ToString() ?? "",

                FacilityType = dr["FacilityType"]?.ToString() ?? "",

                IsActive = dr["IsActive"] != DBNull.Value
                                ? Convert.ToBoolean(dr["IsActive"])
                                : false,

                NinNumber = dr["NinNumber"] != DBNull.Value
                                ? dr["NinNumber"].ToString()
                                : null,

                BlockId = dr["BlockId"] != DBNull.Value
                                ? Convert.ToInt32(dr["BlockId"])
                                : 0,

                IsIntervention = dr["IsIntervention"] != DBNull.Value
                                ? Convert.ToBoolean(dr["IsIntervention"])
                                : false
            };
        }

    }
}