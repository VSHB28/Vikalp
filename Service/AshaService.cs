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

    public async Task<AshaDto?> GetById(int id)
    {
        using var conn = Conn();
        return await conn.QueryFirstOrDefaultAsync<AshaDto>(
            "USP_Asha_CRUD",
            new { Action = "GETBYID", AshaId = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task Insert(AshaDto model, int userId)
    {
        using var conn = Conn();
        if (conn.State != ConnectionState.Open) conn.Open();

        await conn.ExecuteAsync(
            "USP_Asha_CRUD",
            new
            {
                Action = "INSERT",
                AshaName = model.AshaName ?? (object)DBNull.Value,
                AshaMobile = model.AshaMobile ?? (object)DBNull.Value,
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

    public async Task<List<AshaDto>> GetByFilter(int? stateId, int? districtId, int? blockId, int? facilityId)
    {
        List<AshaDto> list = new();

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("USP_Asha_Filter", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StateId", (object?)stateId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", (object?)districtId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockId", (object?)blockId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FacilityId", (object?)facilityId ?? DBNull.Value);

            await con.OpenAsync();
            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                list.Add(new AshaDto
                {
                    AshaId = Convert.ToInt32(dr["AshaId"]),
                    AshaName = dr["AshaName"].ToString(),
                    AshaMobile = dr["AshaMobile"].ToString(),
                    IsActive = Convert.ToBoolean(dr["IsActive"]),
                    AttendedVCAT = Convert.ToBoolean(dr["AttendedVCAT"])
                });
            }
        }

        return list;
    }
}

