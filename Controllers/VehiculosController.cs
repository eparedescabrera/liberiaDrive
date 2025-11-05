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
    public class VehiculosController : Controller
    {
        private readonly DatabaseService _db;

        public VehiculosController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ INDEX - LISTAR TODOS LOS VEHÍCULOS
        // =====================================================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarVehiculos");
            ViewBag.Vehiculos = dt;
            return View();
        }

        // =====================================================
        // 🔍 BUSCAR VEHÍCULO POR PLACA (AJAX)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarPorPlaca(string placa)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@Placa", placa ?? "" } };
                var dt = _db.EjecutarSPDataTable("sp_BuscarVehiculoPorPlaca", parametros);

                var resultados = dt.AsEnumerable().Select(r => new
                {
                    IdVehiculo = r["IdVehiculo"],
                    Marca = r["Marca"].ToString(),
                    Modelo = r["Modelo"].ToString(),
                    Transmision = r["Transmision"].ToString(),
                    Placa = r["Placa"].ToString(),
                    Estado = r["Estado"].ToString()
                });

                return Json(new { success = true, data = resultados });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new Vehiculo { Estado = "Disponible" });
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Create(Vehiculo model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@Marca", model.Marca },
                    { "@Modelo", model.Modelo },
                    { "@Transmision", model.Transmision },
                    { "@Placa", model.Placa },
                    { "@Estado", model.Estado ?? "Disponible" }
                };

                _db.EjecutarSPNonQuery("sp_InsertarVehiculo", parametros);
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
                var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
                var dt = _db.EjecutarSPDataTable("sp_ObtenerVehiculoPorId", parametros);

                if (dt.Rows.Count == 0)
                    return Content("No se encontró el vehículo solicitado.");

                var row = dt.Rows[0];
                var model = new Vehiculo
                {
                    IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                    Marca = row["Marca"].ToString(),
                    Modelo = row["Modelo"].ToString(),
                    Transmision = row["Transmision"].ToString(),
                    Placa = row["Placa"].ToString(),
                    Estado = row["Estado"].ToString()
                };

                return PartialView("_EditPartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al cargar datos: " + ex.Message);
            }
        }

        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
        [HttpPost]
        public IActionResult Edit(Vehiculo model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdVehiculo", model.IdVehiculo },
                    { "@Marca", model.Marca },
                    { "@Modelo", model.Modelo },
                    { "@Transmision", model.Transmision },
                    { "@Placa", model.Placa },
                    { "@Estado", model.Estado }
                };

                _db.EjecutarSPNonQuery("sp_ActualizarVehiculo", parametros);
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
                var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
                var dt = _db.EjecutarSPDataTable("sp_ObtenerVehiculoPorId", parametros);

                if (dt.Rows.Count == 0)
                    return Content("No se encontró el vehículo.");

                var row = dt.Rows[0];
                var model = new Vehiculo
                {
                    IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                    Marca = row["Marca"].ToString(),
                    Modelo = row["Modelo"].ToString(),
                    Transmision = row["Transmision"].ToString(),
                    Placa = row["Placa"].ToString(),
                    Estado = row["Estado"].ToString()
                };

                return PartialView("_DeletePartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al cargar vehículo: " + ex.Message);
            }
        }
// =====================================================
// ✅ DETALLES - GET
// =====================================================
[HttpGet]
public IActionResult Details(int id)
{
    try
    {
        var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
        var dt = _db.EjecutarSPDataTable("sp_ObtenerVehiculoPorId", parametros);

        if (dt.Rows.Count == 0)
            return Content("No se encontró el vehículo solicitado.");

        var row = dt.Rows[0];
        var model = new Vehiculo
        {
            IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
            Marca = row["Marca"].ToString(),
            Modelo = row["Modelo"].ToString(),
            Transmision = row["Transmision"].ToString(),
            Placa = row["Placa"].ToString(),
            Estado = row["Estado"].ToString()
        };

        return PartialView("_DetailsPartial", model);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error al cargar detalles: " + ex.Message);
    }
}

        // =====================================================
        // ✅ ELIMINAR - POST (AJAX con SweetAlert)
        // =====================================================
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
                _db.EjecutarSPNonQuery("sp_EliminarVehiculo", parametros);

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
    }
}
