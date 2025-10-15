using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LiberiaDriveMVC.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        // =====================================================
        // Ejecutar SP que NO devuelve resultados (INSERT, UPDATE, DELETE)
        // =====================================================
        public void EjecutarSPNonQuery(string nombreSP, Dictionary<string, object> parametros)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(nombreSP, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parametros != null)
                {
                    foreach (var p in parametros)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                    }
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // =====================================================
        // Ejecutar SP o consulta que devuelve una tabla
        // =====================================================
       public DataTable EjecutarSPDataTable(string queryOrSP, Dictionary<string, object>? parametros = null, bool? isQuery = null)
{
    DataTable dt = new DataTable();
    using (var connection = new SqlConnection(_connectionString))
    using (var cmd = new SqlCommand(queryOrSP, connection))
    {
        // Detectar automáticamente si es consulta o SP
        if (isQuery == null)
        {
            string lower = queryOrSP.Trim().ToLower();
            if (lower.StartsWith("select") || lower.StartsWith("update") ||
                lower.StartsWith("delete") || lower.StartsWith("insert"))
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;
        }
        else
        {
            cmd.CommandType = isQuery.Value ? CommandType.Text : CommandType.StoredProcedure;
        }

        if (parametros != null)
        {
            foreach (var p in parametros)
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }

        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
    }
    return dt;
}

    }
}
