// Service/SubCentreService.cs

using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

public class SubCentreService : ISubCentreService
{
    private readonly string _connectionString;

    public SubCentreService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    private SqlConnection Conn()
    {
        return new SqlConnection(_connectionString);
    }


    public async Task<string?> GetAllAsync()
    {
        var list = new List<SubCentreDto>();

        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "GetByBlock");
        cmd.Parameters.AddWithValue("@BlockId", DBNull.Value);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(MapSubCentre(reader));
        }

        return System.Text.Json.JsonSerializer.Serialize(list);
    }

    // ================= GET BY BLOCK =================
    public async Task<IEnumerable<object>> GetByBlockAsync(int blockId, int? facilityId)
    {
        var list = new List<SubCentreDto>();

        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "GetByBlock");
        cmd.Parameters.AddWithValue("@BlockId", blockId);

        if (facilityId.HasValue)
            cmd.Parameters.AddWithValue("@FacilityId", facilityId.Value);
        else
            cmd.Parameters.AddWithValue("@FacilityId", DBNull.Value);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(MapSubCentre(reader));
        }

        return list.Cast<object>();
    }

    // ================= GET BY ID =================
    public async Task<string?> GetByIdAsync(int id)
    {
        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "GetById");
        cmd.Parameters.AddWithValue("@SubCentreId", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var dto = MapSubCentre(reader);
           
            return System.Text.Json.JsonSerializer.Serialize(dto);
        }

        return null;
    }

    public async Task AddAsync(SubCentreDto dto)
    {
        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "Insert");

        cmd.Parameters.AddWithValue("@SubCentreId",
    dto.SubCentreId ?? (object)DBNull.Value);


        cmd.Parameters.AddWithValue("@SubCentre", dto.SubCentre);
        cmd.Parameters.AddWithValue("@NinNumber", dto.NinNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsLearningSite", dto.IsLearningSite);
        cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
        cmd.Parameters.AddWithValue("@BlockId", dto.BlockId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FacilityId", dto.FacilityId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AnmId", dto.AnmId ?? (object)DBNull.Value);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // ================= UPDATE =================
    public async Task UpdateAsync(SubCentreDto dto)
    {
        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "Update");
        cmd.Parameters.AddWithValue("@SubCentreId", dto.SubCentreId);
        cmd.Parameters.AddWithValue("@SubCentre", dto.SubCentre);
        cmd.Parameters.AddWithValue("@NinNumber", dto.NinNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsLearningSite", dto.IsLearningSite);
        cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
        cmd.Parameters.AddWithValue("@BlockId", dto.BlockId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FacilityId", dto.FacilityId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AnmId", dto.AnmId ?? (object)DBNull.Value);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // ================= DELETE =================
    public async Task DeleteAsync(int id)
    {
        using var conn = Conn();
        using var cmd = new SqlCommand("USP_SubCentre_CRUD", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Action", "Delete");
        cmd.Parameters.AddWithValue("@SubCentreId", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // ================= MAPPER =================
    private SubCentreDto MapSubCentre(SqlDataReader reader)
    {
        return new SubCentreDto
        {
            SubCentreId = Convert.ToInt32(reader["SubCentreId"]),
            SubCentre = reader["SubCentre"].ToString(),
            NinNumber = reader["NinNumber"]?.ToString(),
            IsLearningSite = Convert.ToBoolean(reader["IsLearningSite"]),
            IsActive = Convert.ToBoolean(reader["IsActive"]),
            BlockId = reader["BlockId"] as int?,
            FacilityId = reader["FacilityId"] as int?,
            AnmId = reader["AnmId"] as int?,
             FacilityName = reader["FacilityName"]?.ToString(),
             BlockName = reader["BlockName"]?.ToString(),
        
            FacilityType = reader["FacilityType"]?.ToString()
        };
    }

    public async Task SaveFacilityProfileAsync(FacilityProfileDto model)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("@ProfileId", model.ProfileId); 
                parameters.Add("@FacilityId", model.FacilityId);
                parameters.Add("@SubCentreId", model.SubCenterId);
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
                    "sp_InsertFacilityProfile",   
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
}

