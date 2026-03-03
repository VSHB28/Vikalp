using Humanizer;
using Microsoft.Data.SqlClient;
using System.Data;
using Vikalp.Models.DTO;
using Vikalp.Service.Interfaces;
public class ChecklistVisitService : IChecklistVisitService
{
    private readonly IConfiguration _config;

    public ChecklistVisitService(IConfiguration config)
    {
        _config = config;
    }

    private string Conn() => _config.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("No connection string");

    public async Task<(IEnumerable<ChecklistVisitDTO> Data, int TotalCount)> GetAllAsync(int userId, int page, int pageSize, int? StateId, int? DistrictId, int? BlockId, int? FacilityId, int? SubCenterId)
    {
        var list = new List<ChecklistVisitDTO>();
        int totalCount = 0;

        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetEventActivities", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Page", page);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        cmd.Parameters.AddWithValue("@StateId", (object?)StateId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DistrictId", (object?)DistrictId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BlockId", (object?)BlockId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FacilityId", (object?)FacilityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SubCenterId", (object?)SubCenterId ?? DBNull.Value);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dto = new ChecklistVisitDTO
            {
                
            };

            // capture total count from COUNT(*) OVER()
            if (!reader.IsDBNull(reader.GetOrdinal("TotalRecords")))
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
            }

            list.Add(dto);
        }

        return (list, totalCount);
    }

    public async Task<ChecklistVisitDTO?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_GetEventActivityById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@EventId", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ChecklistVisitDTO
            {
                
            };
        }

        return null;
    }

    private int? GetNullableInt(SqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
    }

    public async Task<bool> CreateAsync(ChecklistVisitDTO dto, int userId)
    {
        try
        {
            using var conn = new SqlConnection(Conn());

            await conn.OpenAsync(); // ✅ THIS LINE FIXES THE ERROR

            using var cmd = new SqlCommand("sp_CreateEventActivity", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventGuid", Guid.NewGuid().ToString());

           

            cmd.Parameters.AddWithValue("@StateId", (object?)dto.StateId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", (object?)dto.DistrictId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BlockId", (object?)dto.BlockId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FacilityId", (object?)dto.FacilityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubcentreId", (object?)dto.SubcentreId ?? DBNull.Value);

            

            cmd.Parameters.AddWithValue("@CreatedBy", userId);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Optional: log ex.Message
            return false;
        }
    }

    public async Task<bool> UpdateAsync(ChecklistVisitDTO dto, int userId)
    {
        try
        {
            using var con = new SqlConnection(Conn());
            using var cmd = new SqlCommand("sp_UpdateEventActivity", con);

            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue("@StateId", dto.StateId);
            cmd.Parameters.AddWithValue("@DistrictId", dto.DistrictId);
            cmd.Parameters.AddWithValue("@BlockId", dto.BlockId);
            cmd.Parameters.AddWithValue("@FacilityId", dto.FacilityId);
            cmd.Parameters.AddWithValue("@SubCentreId", dto.SubcentreId);

            

            cmd.Parameters.AddWithValue("@UpdatedBy", userId);

            await con.OpenAsync();

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = new SqlConnection(Conn());
        using var cmd = new SqlCommand("sp_DeleteEventActivity", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EventId", id);
        cmd.Parameters.AddWithValue("@DeletedBy", userId);

        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}