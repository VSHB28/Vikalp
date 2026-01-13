using Npgsql;
using System.Data;
using System.Text.Json;

namespace Vikalp.Utilities
{
    public static class DataTableUtils
    {
        public static DataTable ExecuteStoredProcedure(
            string connectionString,
            string procedureName,
            params NpgsqlParameter[] parameters)
        {
            using var conn = new NpgsqlConnection(connectionString);
            using var cmd = new NpgsqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();

            conn.Open();
            adapter.Fill(dt);

            return dt;
        }

        public static int ExecuteStoredProcedureNonQuery(
            string connectionString,
            string procedureName,
            params NpgsqlParameter[] parameters)
        {
            using var conn = new NpgsqlConnection(connectionString);
            using var cmd = new NpgsqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteQuery(
            string connectionString,
            string query,
            params NpgsqlParameter[] parameters)
        {
            using var conn = new NpgsqlConnection(connectionString);
            using var cmd = new NpgsqlCommand(query, conn);

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();

            conn.Open();
            adapter.Fill(dt);

            return dt;
        }

        public static DataSet ExecuteQueryDataSet(
            string connectionString,
            string query,
            params NpgsqlParameter[] parameters)
        {
            using var conn = new NpgsqlConnection(connectionString);
            using var cmd = new NpgsqlCommand(query, conn);

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new NpgsqlDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }

        // =========================
        // JSON HELPERS
        // =========================

        public static string ExecuteStoredProcedureAsJson(
            string connectionString,
            string procedureName,
            params NpgsqlParameter[] parameters)
        {
            var dt = ExecuteStoredProcedure(connectionString, procedureName, parameters);
            return JsonSerializer.Serialize(dt);
        }

        public static string ExecuteStoredProcedureAsJsonWithSchema(
            string connectionString,
            string procedureName,
            params NpgsqlParameter[] parameters)
        {
            var dt = ExecuteStoredProcedure(connectionString, procedureName, parameters);

            var result = new
            {
                Schema = dt.Columns.Cast<DataColumn>().Select(c => new
                {
                    c.ColumnName,
                    DataType = c.DataType.Name
                }),
                Data = dt
            };

            return JsonSerializer.Serialize(result);
        }
    }
}
