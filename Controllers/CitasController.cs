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
            // Actualiza automáticamente las citas vencidas antes de listar
            _db.EjecutarSPNonQuery("sp_ActualizarEstadoCitas", null);

            var dt = _db.EjecutarSPDataTable("sp_ListarCitas", null);
            ViewBag.Citas = dt;
            return View();
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            // No cargamos clientes aquí (lo hará el buscador AJAX)
            return PartialView("_CreatePartial", new Cita());
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Create(Cita model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos del formulario." });

            try
            {
                var param = new Dictionary<string, object>
                {
                    { "@IdCliente", model.IdCliente },
                    { "@TipoExamen", model.TipoExamen },
                    { "@FechaCita", model.FechaCita }
                };

                _db.EjecutarSPNonQuery("sp_InsertarCita", param);
                return Json(new { success = true, message = "Cita registrada correctamente." });
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
            var dt = _db.EjecutarSPDataTable("sp_ObtenerCitaPorId",
                new Dictionary<string, object> { { "@IdCita", id } });

            if (dt.Rows.Count == 0)
                return Content("<div class='alert alert-warning'>Cita no encontrada.</div>");

            var row = dt.Rows[0];

            var cita = new Cita
            {
                IdCita = id,
                IdCliente = Convert.ToInt32(row["IdCliente"]),
                NombreCliente = row["NombreCliente"].ToString(),
                TipoExamen = row["TipoExamen"].ToString(),
                FechaCita = Convert.ToDateTime(row["FechaCita"]),
                Estado = row["Estado"].ToString()
            };

            return PartialView("_EditPartial", cita);
        }

        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Edit(Cita model)
        {
            try
            {
                var param = new Dictionary<string, object>
                {
                    { "@IdCita", model.IdCita },
                    { "@IdCliente", model.IdCliente },
                    { "@TipoExamen", model.TipoExamen },
                    { "@FechaCita", model.FechaCita },
                    { "@Estado", model.Estado }
                };

                _db.EjecutarSPNonQuery("sp_ActualizarCita", param);
                return Json(new { success = true, message = "Cita actualizada correctamente." });
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

                return Json(new { success = true, message = "Cita eliminada correctamente." });
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
            // term es el texto que el usuario escribe en el buscador
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
    }
}
