using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {
        private readonly DatabaseService _db;

        public CitasController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // LISTAR CITAS
        // =====================================================
        public IActionResult Index()
        {
            var citasDt = _db.EjecutarSPDataTable("sp_ListarCitas", null);

            var citas = new List<Cita>();
            foreach (DataRow row in citasDt.Rows)
            {
                citas.Add(MapearCita(row));
            }

            return View(citas);
        }

        // =====================================================
        // CREAR CITA (GET)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View(new Cita { FechaCita = DateTime.Now, Estado = "Pendiente" });
        }

        // =====================================================
        // CREAR CITA (POST)
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Cita model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var parametros = new Dictionary<string, object>
            {
                { "@IdCliente", model.IdCliente },
                { "@TipoExamen", model.TipoExamen },
                { "@FechaCita", model.FechaCita },
                { "@Estado", model.Estado ?? "Pendiente" }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarCita", parametros);
                TempData["Mensaje"] = "✅ Cita creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // =====================================================
        // EDITAR CITA (GET)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarCitas", null);
            var row = BuscarCitaPorId(dt, id);

            if (row == null)
                return NotFound();

            var cita = MapearCita(row);
            return View(cita);
        }

        // =====================================================
        // EDITAR CITA (POST)
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit([FromForm] Cita model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var parametros = new Dictionary<string, object>
            {
                { "@IdCita", model.IdCita },
                { "@IdCliente", model.IdCliente },
                { "@TipoExamen", model.TipoExamen },
                { "@FechaCita", model.FechaCita },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarCita", parametros);
                TempData["Mensaje"] = "✏️ Cita actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // =====================================================
        // ELIMINAR CITA
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarCitas", null);
            var row = BuscarCitaPorId(dt, id);

            if (row == null)
                return NotFound();

            var cita = MapearCita(row);
            return View(cita);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdCita)
        {
            var parametros = new Dictionary<string, object> { { "@IdCita", IdCita } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarCita", parametros);
                TempData["Mensaje"] = "🗑️ Cita eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // =====================================================
        // MÉTODOS AUXILIARES
        // =====================================================
        private static DataRow BuscarCitaPorId(DataTable dt, int id)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdCita"]) == id)
                    return r;
            }
            return null;
        }

        private static Cita MapearCita(DataRow row)
        {
            return new Cita
            {
                IdCita = Convert.ToInt32(row["IdCita"]),
                IdCliente = Convert.ToInt32(row["IdCliente"]),
                TipoExamen = row["TipoExamen"].ToString(),
                FechaCita = Convert.ToDateTime(row["FechaCita"]),
                Estado = row["Estado"]?.ToString()
            };
        }
    }
}
