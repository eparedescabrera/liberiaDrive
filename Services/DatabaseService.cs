using System.Data;
using Microsoft.Data.SqlClient;

namespace LiberiaDriveMVC.Services
{
    public class DatabaseService
    {
        private readonly string _connectionStringOLTP;
        private readonly string _connectionStringDW;

        public DatabaseService(IConfiguration cfg)
        {
            _connectionStringOLTP = cfg.GetConnectionString("DefaultConnection")!;
            _connectionStringDW = cfg.GetConnectionString("DWConnection")!;
        }

        // =====================================================
        // ✅ Ejecutar SP o consulta en OLTP (LiberiaDriveDB)
        // =====================================================
        public DataTable EjecutarSPDataTable(string queryOrSP, Dictionary<string, object>? parametros = null, bool isSql = false)
        {
            using var cn = new SqlConnection(_connectionStringOLTP);
            using var cmd = new SqlCommand(queryOrSP, cn);
            cmd.CommandType = isSql ? CommandType.Text : CommandType.StoredProcedure;

            if (parametros != null)
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            cn.Open();
            da.Fill(dt);
            return dt;
        }

        public int EjecutarSPNonQuery(string queryOrSP, Dictionary<string, object>? parametros = null, bool isSql = false)
        {
            using var cn = new SqlConnection(_connectionStringOLTP);
            using var cmd = new SqlCommand(queryOrSP, cn);
            cmd.CommandType = isSql ? CommandType.Text : CommandType.StoredProcedure;

            if (parametros != null)
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            cn.Open();
            return cmd.ExecuteNonQuery();
        }

        // =====================================================
        // ✅ Consultas SQL directas (solo OLTP)
        // =====================================================
        public DataTable EjecutarQuery(string query, Dictionary<string, object>? parametros = null)
        {
            using var cn = new SqlConnection(_connectionStringOLTP);
            using var cmd = new SqlCommand(query, cn);
            cmd.CommandType = CommandType.Text;

            if (parametros != null)
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // =====================================================
        // 🧩 NUEVOS MÉTODOS: conexión al DW (LiberiaDriveDW)
        // =====================================================
        public DataTable EjecutarSPDataTableDW(string nombreSP, Dictionary<string, object>? parametros = null)
        {
            using var cn = new SqlConnection(_connectionStringDW);
            using var cmd = new SqlCommand(nombreSP, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parametros != null)
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void EjecutarSPNonQueryDW(string nombreSP, Dictionary<string, object>? parametros = null)
        {
            using var cn = new SqlConnection(_connectionStringDW);
            using var cmd = new SqlCommand(nombreSP, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parametros != null)
                foreach (var p in parametros)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
