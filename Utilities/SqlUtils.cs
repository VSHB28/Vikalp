using Microsoft.Data.SqlClient;
using System.Data;

namespace Vikalp.Utilities;

public static class SqlUtils
{
    public static DataTable ExecuteSP(string connectionString, string spName, SqlParameter[] parameters)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand(spName, con))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }

}