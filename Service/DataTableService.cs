using Npgsql;
using Vikalp.Utilities;
using System.Data;
namespace Vikalp.Services;

public class DataTableService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataTableService> _logger;

    public DataTableService(IConfiguration configuration, ILogger<DataTableService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string not found");

    public string GetConnectionString() => ConnectionString;

    public DataTable ExecuteStoredProcedure(string procedureName, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedure(ConnectionString, procedureName, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {ProcedureName}", procedureName);
            throw;
        }
    }

    public int ExecuteStoredProcedureNonQuery(string procedureName, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedureNonQuery(ConnectionString, procedureName, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure (non query): {ProcedureName}", procedureName);
            throw;
        }
    }

    public DataTable ExecuteStoredProcedure(string procedureName)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedure(ConnectionString, procedureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {ProcedureName}", procedureName);
            throw;
        }
    }

    public DataTable ExecuteQuery(string query, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteQuery(ConnectionString, query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Query}", query);
            throw;
        }
    }

    public DataSet ExecuteQueryDataSet(string query, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteQueryDataSet(ConnectionString, query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query DataSet: {Query}", query);
            throw;
        }
    }

    public string ExecuteStoredProcedureAsJson(string procedureName, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedureAsJson(ConnectionString, procedureName, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure as JSON: {ProcedureName}", procedureName);
            throw;
        }
    }

    public string ExecuteStoredProcedureAsJson(string procedureName)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedureAsJson(ConnectionString, procedureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure as JSON: {ProcedureName}", procedureName);
            throw;
        }
    }

    public string ExecuteStoredProcedureAsJsonWithSchema(string procedureName, params NpgsqlParameter[] parameters)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedureAsJsonWithSchema(ConnectionString, procedureName, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure as JSON with schema: {ProcedureName}", procedureName);
            throw;
        }
    }

    public string ExecuteStoredProcedureAsJsonWithSchema(string procedureName)
    {
        try
        {
            return DataTableUtils.ExecuteStoredProcedureAsJsonWithSchema(ConnectionString, procedureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure as JSON with schema: {ProcedureName}", procedureName);
            throw;
        }
    }
}
