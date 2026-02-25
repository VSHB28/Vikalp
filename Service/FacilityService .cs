using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
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
                                : false,
                ProfileId = dr["ProfileId"] != DBNull.Value
                                ? Convert.ToInt32(dr["ProfileId"])
                                : 0,
                HrId = dr["HrId"] != DBNull.Value
                                ? Convert.ToInt32(dr["HrId"])
                                : 0
            };
        }

        public async Task SaveFacilityProfileAsync(FacilityProfileDto model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@ProfileId", model.ProfileId); // 👈 Important
                    parameters.Add("@FacilityId", model.FacilityId);
                    parameters.Add("@SubCenterId", model.SubCenterId);
                    parameters.Add("@PopulationCoveredbyPHC", model.PopulationCoveredbyPHC);
                    parameters.Add("@NumberofHSC", model.NumberofHSC);
                    parameters.Add("@PopulationCoveredPHC_HWC", model.PopulationCoveredPHC_HWC);
                    parameters.Add("@PopulationCoveredbyHWC", model.PopulationCoveredbyHWC);
                    parameters.Add("@AverageOPDperDay", model.AverageOPDperDay);
                    parameters.Add("@NearestFacilityReferral", model.NearestFacilityReferral);
                    parameters.Add("@DistancefromPHC", model.DistancefromPHC);
                    parameters.Add("@IsDeliveryPoint", model.IsDeliveryPoint);
                    parameters.Add("@AvgdeliveryperMonth", model.AvgdeliveryperMonth);
                    parameters.Add("@DistancefromDH", model.DistancefromDH);
                    parameters.Add("@IsSeparateSpaceforFp", model.IsSeparateSpaceforFp);
                    parameters.Add("@CreatedBy", model.CreatedBy);

                    await connection.ExecuteAsync(
                        "sp_InsertFacilityProfile",   // renamed properly
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        public async Task<FacilityProfileDto> GetFacilityProfileAsync(int profileId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<FacilityProfileDto>(
                    "usp_GetFacilityProfile",
                    new { ProfileId = profileId },
                    commandType: CommandType.StoredProcedure);
            }
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
    }
}