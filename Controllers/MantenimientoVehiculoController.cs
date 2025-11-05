using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace LiberiaDriveMVC.Controllers
{
    public class MantenimientoVehiculoController : Controller
    {
        private readonly DatabaseService _db;

        public MantenimientoVehiculoController(DatabaseService db)
        {
            _db = db;
        }

        // ======================== INDEX ========================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarMantenimientos", null);
            ViewBag.Mantenimientos = dt;
            return View();
        }

        // ======================== CREATE - GET ========================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new MantenimientoVehiculo
            {
                FechaMantenimiento = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        // ======================== CREATE - POST ========================
        [HttpPost]
        public IActionResult Create(MantenimientoVehiculo model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdVehiculo", model.IdVehiculo },
                    { "@FechaMantenimiento", model.FechaMantenimiento.ToDateTime(TimeOnly.MinValue) },
                    { "@TipoMantenimiento", model.TipoMantenimiento },
                    { "@Costo", model.Costo },
                    { "@Descripcion", model.Descripcion ?? "" }
                };

                _db.EjecutarSPNonQuery("sp_InsertarMantenimiento", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error general: " + ex.Message });
            }
        }

        // ======================== EDIT - GET ========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdMantenimiento", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerMantenimientoPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("No se encontró el mantenimiento.");

            var row = dt.Rows[0];
            var model = new MantenimientoVehiculo
            {
                IdMantenimiento = Convert.ToInt32(row["IdMantenimiento"]),
                IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                FechaMantenimiento = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaMantenimiento"])),
                TipoMantenimiento = row["TipoMantenimiento"].ToString()!,
                Costo = Convert.ToDecimal(row["Costo"]),
                Descripcion = row["Descripcion"].ToString(),
                IdVehiculoNavigation = new Vehiculo { Marca = row["Vehiculo"].ToString() }
            };

            return PartialView("_EditPartial", model);
        }

        // ======================== EDIT - POST ========================
        [HttpPost]
        public IActionResult Edit(MantenimientoVehiculo model)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@IdMantenimiento", model.IdMantenimiento },
                { "@IdVehiculo", model.IdVehiculo },
                { "@FechaMantenimiento", model.FechaMantenimiento.ToDateTime(TimeOnly.MinValue) },
                { "@TipoMantenimiento", model.TipoMantenimiento },
                { "@Costo", model.Costo },
                { "@Descripcion", model.Descripcion ?? "" }
            };

            _db.EjecutarSPNonQuery("sp_ActualizarMantenimiento", parametros);
            return Json(new { success = true });
        }

        // ======================== DELETE ========================
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdMantenimiento", id } };
            _db.EjecutarSPNonQuery("sp_EliminarMantenimiento", parametros);
            return Json(new { success = true });
        }
    }
}
