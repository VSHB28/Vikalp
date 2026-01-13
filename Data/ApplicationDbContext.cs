using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using Vikalp.Models;

namespace Vikalp.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _config;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config)
        : base(options)
    {
        _config = config;
    }

    public DbSet<ApiUser> ApiUsers { get; set; }
    public DbSet<TblAshaOrientation> TblAshaOrientations { get; set; }
    private string ConnectionString =>
        _config.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection is not configured.");

    public async Task<string> ExecuteJsonAsync(string sql, params NpgsqlParameter[] parameters)
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await using var cmd = new NpgsqlCommand(sql, conn)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 60
        };

        if (parameters != null && parameters.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString() ?? "[]";
    }

    public async Task ExecuteProcedureAsync(string sql, params NpgsqlParameter[] parameters)
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await using var cmd = new NpgsqlCommand(sql, conn)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };

        if (parameters != null && parameters.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
