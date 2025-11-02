using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LiberiaDriveMVC.Controllers
{
    public class SesionPracticaController : Controller
    {
        private readonly DatabaseService _db;

        public SesionPracticaController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ INDEX (LISTAR)
        // =====================================================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarSesionesPractica", null);
            ViewBag.Sesiones = dt;
            return View();
        }

        // =====================================================
        // ✅ CREATE (GET)
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new Models.SesionPractica());
        }

        // =====================================================
        // ✅ CREATE (POST)
        // =====================================================
        [HttpPost]
        public JsonResult Create(Models.SesionPractica model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@IdCliente", model.IdCliente},
                    {"@IdInstructor", model.IdInstructor},
                    {"@IdVehiculo", model.IdVehiculo},
                    {"@FechaSesion", model.FechaSesion},
                    {"@Estado", model.Estado ?? "Programada"},
                    {"@Calificacion", model.Calificacion ?? 0}
                };

                _db.EjecutarSPNonQuery("sp_InsertarSesionPractica", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        // =====================================================
        // ✅ EDIT (GET)
        // =====================================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdSesionPractica", id } };
            var dt = _db.EjecutarQuery($"SELECT * FROM SesionPractica WHERE IdSesionPractica = {id}", null);

            if (dt.Rows.Count == 0)
                return NotFound();

            var row = dt.Rows[0];
            var model = new Models.SesionPractica
            {
                IdSesionPractica = Convert.ToInt32(row["IdSesionPractica"]),
                IdCliente = Convert.ToInt32(row["IdCliente"]),
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                FechaSesion = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaSesion"])),
                Estado = row["Estado"].ToString(),
                Calificacion = row["Calificacion"] != DBNull.Value ? Convert.ToDecimal(row["Calificacion"]) : null
            };

            return PartialView("_EditPartial", model);
        }

        // =====================================================
        // ✅ EDIT (POST)
        // =====================================================
        [HttpPost]
        public JsonResult Edit(Models.SesionPractica model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@IdSesionPractica", model.IdSesionPractica},
                    {"@IdCliente", model.IdCliente},
                    {"@IdInstructor", model.IdInstructor},
                    {"@IdVehiculo", model.IdVehiculo},
                    {"@FechaSesion", model.FechaSesion},
                    {"@Estado", model.Estado},
                    {"@Calificacion", model.Calificacion ?? 0}
                };

                _db.EjecutarSPNonQuery("sp_ActualizarSesionPractica", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        // =====================================================
        // ✅ DELETE (GET)
        // =====================================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarQuery($"SELECT * FROM SesionPractica WHERE IdSesionPractica = {id}", null);
            if (dt.Rows.Count == 0)
                return NotFound();

            var model = new Models.SesionPractica
            {
                IdSesionPractica = Convert.ToInt32(dt.Rows[0]["IdSesionPractica"])
            };

            return PartialView("_DeletePartial", model);
        }

        // =====================================================
        // ✅ DELETE (POST)
        // =====================================================
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@IdSesionPractica", id } };
                _db.EjecutarSPNonQuery("sp_EliminarSesionPractica", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ BUSCAR INSTRUCTORES ACTIVOS (para Select2)
        // =====================================================
        [HttpGet]
        public JsonResult BuscarInstructores(string term)
        {
            var parametros = new Dictionary<string, object> { { "@term", term } };
            var dt = _db.EjecutarSPDataTable("sp_BuscarInstructoresActivos", parametros);

            var lista = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new { id = row["id"], text = row["text"].ToString() });
            }

            return Json(lista);
        }
    }
}
