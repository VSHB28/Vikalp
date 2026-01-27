
using Microsoft.Data.SqlClient;
using Vikalp.Service.Interfaces;

public class LocationBlockService : ILocationBlockService
{
    private readonly IConfiguration _configuration;

    public LocationBlockService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    private LocationBlockDto Map(SqlDataReader reader)
    {
        return new LocationBlockDto
        {
            BlockId = Convert.ToInt32(reader["BlockId"]),
            BlockName = reader["BlockName"].ToString() ?? "",
            BlockCode = reader["BlockCode"].ToString() ?? "",
            IsActive = Convert.ToBoolean(reader["IsActive"]),
            RegionId = Convert.ToInt32(reader["RegionId"]),
            DistrictId = Convert.ToInt32(reader["DistrictId"]),
            IsAspirational = Convert.ToBoolean(reader["IsAspirational"])
        };
    }

    public async Task<IEnumerable<LocationBlockDto>> GetAllAsync()
    {
        var list = new List<LocationBlockDto>();

        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "GetAll");

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    public async Task<LocationBlockDto?> GetByIdAsync(int blockId)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "GetById");
        cmd.Parameters.AddWithValue("@BlockId", blockId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<IEnumerable<LocationBlockDto>> GetBlocksByDistrictAsync(int districtId)
    {
        var list = new List<LocationBlockDto>();

        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "GetByDistrict");
        cmd.Parameters.AddWithValue("@DistrictId", districtId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    public async Task AddAsync(LocationBlockDto dto)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "Insert");
        cmd.Parameters.AddWithValue("@BlockName", dto.BlockName);
        cmd.Parameters.AddWithValue("@BlockCode", dto.BlockCode);
        cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
        cmd.Parameters.AddWithValue("@RegionId", dto.RegionId);
        cmd.Parameters.AddWithValue("@DistrictId", dto.DistrictId);
        cmd.Parameters.AddWithValue("@IsAspirational", dto.IsAspirational);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(LocationBlockDto dto)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "Update");
        cmd.Parameters.AddWithValue("@BlockId", dto.BlockId);
        cmd.Parameters.AddWithValue("@BlockName", dto.BlockName);
        cmd.Parameters.AddWithValue("@BlockCode", dto.BlockCode);
        cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
        cmd.Parameters.AddWithValue("@RegionId", dto.RegionId);
        cmd.Parameters.AddWithValue("@DistrictId", dto.DistrictId);
        cmd.Parameters.AddWithValue("@IsAspirational", dto.IsAspirational);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int blockId)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("USP_LocationBlock_CRUD", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", "Delete");
        cmd.Parameters.AddWithValue("@BlockId", blockId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
