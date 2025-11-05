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
    public class SesionPracticaController : Controller
    {
        private readonly DatabaseService _db;

        public SesionPracticaController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ INDEX - LISTAR TODAS LAS SESIONES
        // =====================================================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarSesionesPractica", null);
            ViewBag.Sesiones = dt;
            return View();
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new SesionPractica
            {
                FechaSesion = DateOnly.FromDateTime(DateTime.Today),
                Estado = "Programada"
            });
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Create(SesionPractica model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdCliente", model.IdCliente },
                    { "@IdInstructor", model.IdInstructor },
                    { "@IdVehiculo", model.IdVehiculo },
                    { "@FechaSesion", model.FechaSesion.ToDateTime(TimeOnly.MinValue) },
                    { "@Estado", model.Estado ?? "Programada" }
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
                return Json(new { success = false, message = "Error general: " + ex.Message });
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
                var parametros = new Dictionary<string, object> { { "@IdSesionPractica", id } };
                var dt = _db.EjecutarSPDataTable("sp_ObtenerSesionPracticaPorId", parametros);

                if (dt.Rows.Count == 0)
                    return Content("No se encontró la sesión práctica.");

                var row = dt.Rows[0];
                var model = new SesionPractica
                {
                    IdSesionPractica = Convert.ToInt32(row["IdSesionPractica"]),
                    IdCliente = Convert.ToInt32(row["IdCliente"]),
                    IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                    IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                    FechaSesion = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaSesion"])),
                    Estado = row["Estado"].ToString(),
                    IdClienteNavigation = new Cliente { Nombre = row["NombreCliente"].ToString() },
                    IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() },
                    IdVehiculoNavigation = new Vehiculo { Marca = row["MarcaVehiculo"].ToString() }
                };

                return PartialView("_EditPartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Edit(SesionPractica model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdSesionPractica", model.IdSesionPractica },
                    { "@IdCliente", model.IdCliente },
                    { "@IdInstructor", model.IdInstructor },
                    { "@IdVehiculo", model.IdVehiculo },
                    { "@FechaSesion", model.FechaSesion.ToDateTime(TimeOnly.MinValue) },
                    { "@Estado", model.Estado ?? "Programada" }
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
                return Json(new { success = false, message = "Error general: " + ex.Message });
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
                var parametros = new Dictionary<string, object> { { "@IdSesionPractica", id } };
                var dt = _db.EjecutarSPDataTable("sp_ObtenerSesionPracticaPorId", parametros);

                if (dt.Rows.Count == 0)
                    return Content("No se encontró la sesión práctica.");

                var row = dt.Rows[0];
                var model = new SesionPractica
                {
                    IdSesionPractica = Convert.ToInt32(row["IdSesionPractica"]),
                    IdCliente = Convert.ToInt32(row["IdCliente"]),
                    IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                    IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                    FechaSesion = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaSesion"])),
                    Estado = row["Estado"].ToString(),
                    IdClienteNavigation = new Cliente { Nombre = row["NombreCliente"].ToString() },
                    IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() },
                    IdVehiculoNavigation = new Vehiculo { Marca = row["MarcaVehiculo"].ToString() }
                };

                return PartialView("_DeletePartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar datos: {ex.Message}");
            }
        }

        // =====================================================
        // ✅ ELIMINAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@IdSesionPractica", id } };
                _db.EjecutarSPNonQuery("sp_EliminarSesionPractica", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // 🔍 BUSCAR CLIENTES (AJAX)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarClientes(string term)
        {
            var parametros = new Dictionary<string, object> { { "@Texto", term ?? "" } };
            var dt = _db.EjecutarSPDataTable("sp_BuscarClientes", parametros);

            var resultados = dt.AsEnumerable().Select(r => new
            {
                id = r["IdCliente"],
                text = r["NombreCompleto"].ToString()
            });

            return Json(resultados);
        }
[HttpGet]
public IActionResult BuscarInstructores(string term)
{
    var parametros = new Dictionary<string, object> { { "@term", term ?? "" } };
    var dt = _db.EjecutarSPDataTable("sp_BuscarInstructoresActivos", parametros);

    var resultados = dt.AsEnumerable().Select(r => new
    {
        id = r["id"],
        text = r["text"].ToString()
    });

    return Json(resultados);
}

        // =====================================================
        // 🔍 BUSCAR INSTRUCTORES ACTIVOS (AJAX)
        // =====================================================
       [HttpGet]
public IActionResult BuscarInstructoresActivos(string term)
{
    var parametros = new Dictionary<string, object> { { "@term", term ?? "" } };
    var dt = _db.EjecutarSPDataTable("sp_BuscarInstructoresActivos", parametros);

    var resultados = dt.AsEnumerable().Select(r => new
    {
        id = r["id"],
        text = r["text"].ToString()
    });

    return Json(resultados);
}

        // =====================================================
        // 🔍 BUSCAR VEHÍCULOS (AJAX)
        // =====================================================
// =====================================================
// 🔍 BUSCAR VEHÍCULOS DISPONIBLES (solo los que pueden usarse)
// =====================================================
[HttpGet]
public IActionResult BuscarVehiculos(string term)
{
    var parametros = new Dictionary<string, object> { { "@term", term ?? "" } };
    var dt = _db.EjecutarSPDataTable("sp_BuscarVehiculosDisponibles", parametros);

    var resultados = dt.AsEnumerable().Select(r => new
    {
        id = r["id"],
        text = r["text"].ToString()
    });

    return Json(resultados);
}


    }
}
