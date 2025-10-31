using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LiberiaDriveMVC.Controllers
{
    public class CitasController : Controller
    {
        private readonly DatabaseService _db;

        public CitasController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ LISTAR CITAS
        // =====================================================
        public IActionResult Index()
        {
            try
            {
                // Actualiza automáticamente las citas vencidas antes de listar
                _db.EjecutarSPNonQuery("sp_ActualizarEstadoCitas", null);

                var dt = _db.EjecutarSPDataTable("sp_ListarCitas", null);
                ViewBag.Citas = dt;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "❌ Error al cargar las citas: " + ex.Message;
                return View();
            }
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            // El buscador AJAX cargará los clientes dinámicamente
            return PartialView("_CreatePartial", new Cita());
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Create(Cita model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "❌ Datos inválidos del formulario." });

            try
            {
                // 🕒 Validar fecha anterior
                if (model.FechaCita.Date < DateTime.Today)
                    return Json(new { success = false, message = "❌ No se puede registrar una cita con fecha anterior al día actual." });

                // 🔍 Validar duplicado o teórico pendiente usando SP
                var validarParams = new Dictionary<string, object>
                {
                    { "@IdCliente", model.IdCliente },
                    { "@FechaCita", model.FechaCita },
                    { "@TipoExamen", model.TipoExamen }
                };

                var dtValidacion = _db.EjecutarSPDataTable("sp_VerificarCitaDuplicada", validarParams);

                if (dtValidacion.Rows.Count > 0)
                {
                    var mensaje = dtValidacion.Rows[0]["Mensaje"].ToString();
                    var tipo = dtValidacion.Rows[0].Table.Columns.Contains("TipoError")
                        ? dtValidacion.Rows[0]["TipoError"].ToString()
                        : "";

                    if (tipo == "DUPLICADA" || tipo == "TEORICO_PENDIENTE")
                        return Json(new { success = false, message = mensaje });
                }

                // 💾 Registrar cita
                var param = new Dictionary<string, object>
                {
                    { "@IdCliente", model.IdCliente },
                    { "@TipoExamen", model.TipoExamen },
                    { "@FechaCita", model.FechaCita }
                };

                _db.EjecutarSPNonQuery("sp_InsertarCita", param);
                return Json(new { success = true, message = "✅ Cita registrada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ EDITAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ObtenerCitaPorId",
                    new Dictionary<string, object> { { "@IdCita", id } });

                if (dt.Rows.Count == 0)
                    return Content("<div class='alert alert-warning'>Cita no encontrada.</div>");

                var row = dt.Rows[0];

                var cita = new Cita
                {
                    IdCita = id,
                    IdCliente = Convert.ToInt32(row["IdCliente"]),
                    TipoExamen = row["TipoExamen"].ToString(),
                    FechaCita = Convert.ToDateTime(row["FechaCita"]),
                    Estado = row["Estado"].ToString(),
                    ClienteNombre = row.Table.Columns.Contains("NombreCliente") ? row["NombreCliente"].ToString() : ""
                };

                return PartialView("_EditPartial", cita);
            }
            catch (Exception ex)
            {
                return Content($"<div class='text-danger text-center'>❌ Error: {ex.Message}</div>");
            }
        }

        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Edit(Cita model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "❌ Datos inválidos del formulario." });

                // Validación de fecha anterior
                if (model.FechaCita.Date < DateTime.Today)
                    return Json(new { success = false, message = "❌ No se puede modificar la cita con una fecha anterior al día actual." });

                var parametros = new Dictionary<string, object>
                {
                    { "@IdCita", model.IdCita },
                    { "@IdCliente", model.IdCliente },
                    { "@TipoExamen", model.TipoExamen },
                    { "@FechaCita", model.FechaCita },
                    { "@Estado", model.Estado }
                };

                _db.EjecutarSPNonQuery("sp_ActualizarCita", parametros);
                return Json(new { success = true, message = "✅ Cita actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ ELIMINAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ObtenerCitaPorId",
                    new Dictionary<string, object> { { "@IdCita", id } });

                if (dt.Rows.Count == 0)
                    return Content("<div class='alert alert-warning text-center'>Cita no encontrada.</div>");

                var row = dt.Rows[0];

                var cita = new Cita
                {
                    IdCita = id,
                    NombreCliente = row["NombreCliente"].ToString(),
                    TipoExamen = row["TipoExamen"].ToString(),
                    FechaCita = Convert.ToDateTime(row["FechaCita"]),
                    Estado = row["Estado"].ToString()
                };

                return PartialView("_DeletePartial", cita);
            }
            catch (Exception ex)
            {
                return Content($"<div class='text-danger text-center'>❌ Error: {ex.Message}</div>");
            }
        }

        // =====================================================
        // ✅ ELIMINAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult DeleteConfirmed(int IdCita)
        {
            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarCita",
                    new Dictionary<string, object> { { "@IdCita", IdCita } });

                return Json(new { success = true, message = "🗑️ Cita eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // 🔍 BUSCADOR AJAX DE CLIENTES (para Select2)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarClientes(string term)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@Texto", term ?? "" }
            };

            var dt = _db.EjecutarSPDataTable("sp_BuscarClientes", parametros);

            var resultados = dt.AsEnumerable().Select(r => new
            {
                id = r["IdCliente"],
                text = r["NombreCompleto"].ToString()
            });

            return Json(resultados);
        }

        // =====================================================
        // 🔧 UTILIDAD: Obtener ID de cliente por nombre
        // =====================================================
        private int ObtenerIdClienteDesdeNombre(string nombreCompleto)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string nombre = row["NombreCompleto"].ToString();
                if (string.Equals(nombre, nombreCompleto, StringComparison.OrdinalIgnoreCase))
                    return Convert.ToInt32(row["IdCliente"]);
            }

            return 0;
        }
    }
}
