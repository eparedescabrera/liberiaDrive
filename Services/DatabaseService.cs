using System.Data;
using Microsoft.Data.SqlClient;

namespace LiberiaDriveMVC.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration cfg)
        {
            _connectionString = cfg.GetConnectionString("DefaultConnection")!;
        }

        // ✅ DataTable: puede ejecutar SP o SQL directo
        public DataTable EjecutarSPDataTable(string queryOrSP, Dictionary<string, object>? parametros = null, bool isSql = false)
{
    using var cn = new SqlConnection(_connectionString);
    using var cmd = new SqlCommand(queryOrSP, cn);
    cmd.CommandType = isSql ? CommandType.Text : CommandType.StoredProcedure;

    if (parametros != null)
    {
        foreach (var p in parametros)
            cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
    }

    using var da = new SqlDataAdapter(cmd);
    var dt = new DataTable();

    cn.Open();
    Console.WriteLine($"🟢 Ejecutando {queryOrSP}");
    da.Fill(dt);
    Console.WriteLine($"✅ Registros devueltos: {dt.Rows.Count}");
    return dt;
}

        

        // ✅ NonQuery: puede ejecutar SP o SQL directo (¡la nueva sobrecarga!)
        public int EjecutarSPNonQuery(string queryOrSP, Dictionary<string, object>? parametros = null, bool isSql = false)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(queryOrSP, cn);
            cmd.CommandType = isSql ? CommandType.Text : CommandType.StoredProcedure;

            if (parametros != null)
            {
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            cn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
