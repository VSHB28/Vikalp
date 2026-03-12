namespace Vikalp.Service;

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;

    public class AshaService : IAshaService
    {
   
    private readonly string _connectionString;

    public AshaService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    private SqlConnection Conn()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<List<AshaDto>> GetAllAsha()
    {
        using var conn = Conn();
        var result = await conn.QueryAsync<AshaDto>(
            "USP_Asha_CRUD",
            new { Action = "GET" },
            commandType: CommandType.StoredProcedure
        );
        return result.ToList();
    }

    public async Task<AshaDto> GetById(int id)
    {
        using var conn = Conn();

        var data = await conn.QueryFirstOrDefaultAsync<AshaDto>(
            "USP_Asha_CRUD",
            new
            {
                Action = "GETBYID",
                AshaId = id
            },
            commandType: CommandType.StoredProcedure
        );

        return data;
    }

    public async Task Insert(AshaDto model, int userId)
    {
        using var conn = Conn();

        await conn.ExecuteAsync(
            "USP_Asha_CRUD",
            new
            {
                Action = "INSERT",
                AshaName = model.AshaName ?? (object)DBNull.Value,
                AshaMobile = model.AshaMobile ?? (object)DBNull.Value,
                FacilityId = model.FacilityId ?? (object)DBNull.Value,
                SubCentreId = model.SubCentreId ?? (object)DBNull.Value,
                IsActive = model.IsActive,
                AttendedVCAT = model.AttendedVCAT,
                IsIntervention = model.IsIntervention,
                CreatedBy = userId
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task Update(AshaDto model, int userId)
    {
        using var conn = Conn();
        if (conn.State != ConnectionState.Open) conn.Open();

        await conn.ExecuteAsync(
            "USP_Asha_CRUD",
            new
            {
                Action = "UPDATE",
                model.AshaId,
                model.AshaName,
                model.AshaMobile,
                model.FacilityId,
                model.SubCentreId,
                model.IsActive,
                model.AttendedVCAT,
                model.IsIntervention,
                UpdatedBy = userId
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = Conn();
        if (conn.State != ConnectionState.Open)
            conn.Open();

        await conn.ExecuteAsync(
            "USP_Asha_CRUD",
            new
            {
                Action = "DELETE",
                AshaId = id
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public Task GetByBlockAsync(int blockId, int? facilityId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AshaDto>> GetByFilter(int? stateId, int? districtId, int? blockId, int? facilityId, int? subCentreId)
    {
        List<AshaDto> list = new();

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("USP_Asha_Filter", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@StateId", SqlDbType.Int).Value = (object?)stateId ?? DBNull.Value;
            cmd.Parameters.Add("@DistrictId", SqlDbType.Int).Value = (object?)districtId ?? DBNull.Value;
            cmd.Parameters.Add("@BlockId", SqlDbType.Int).Value = (object?)blockId ?? DBNull.Value;
            cmd.Parameters.Add("@FacilityId", SqlDbType.Int).Value = (object?)facilityId ?? DBNull.Value;
            cmd.Parameters.Add("@SubCentreId", SqlDbType.Int).Value = (object?)subCentreId ?? DBNull.Value;


            await con.OpenAsync();
            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(new AshaDto
                {
                    AshaId = Convert.ToInt32(dr["AshaId"]),
                    AshaName = dr["AshaName"]?.ToString(),
                    AshaMobile = dr["AshaMobile"]?.ToString(),
                    IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"]),
                    AttendedVCAT = dr["AttendedVCAT"] != DBNull.Value && Convert.ToBoolean(dr["AttendedVCAT"])
                });
            }
        }

        return list;
    }




    public async Task<(List<AshaDto> Data, int TotalCount)> GetAllAsha(int page, int pageSize)
    {
        using var conn = Conn();

        var result = await conn.QueryMultipleAsync(
            "USP_Asha_Pagination",
            new { PageNumber = page, PageSize = pageSize },
            commandType: CommandType.StoredProcedure
        );

        var list = result.Read<AshaDto>().ToList();
        int totalCount = result.Read<int>().FirstOrDefault();

        return (list, totalCount);
    }
}

