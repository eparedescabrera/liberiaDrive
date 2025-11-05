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
    public class PagoController : Controller
    {
        private readonly DatabaseService _db;

        public PagoController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ INDEX - LISTAR PAGOS
        // =====================================================
        public IActionResult Index()
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ListarPagos", null);
                ViewBag.Pagos = dt;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al listar pagos: " + ex.Message;
                return View();
            }
        }

        // =====================================================
        // ✅ BUSCAR INSCRIPCIONES (SELECT2 AJAX)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarInscripciones(string Texto)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@Texto", Texto ?? "" } };
                var dt = _db.EjecutarSPDataTable("sp_BuscarInscripciones", parametros);

                var resultados = dt.AsEnumerable().Select(r => new
                {
                    id = r["IdInscripcion"],
                    text = r["Descripcion"].ToString(),
                    costo = Convert.ToDecimal(r["Costo"])
                });

                return Json(resultados);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al buscar inscripciones: " + ex.Message });
            }
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return PartialView("_CreatePartial", new Pago
                {
                    FechaPago = DateOnly.FromDateTime(DateTime.Today),
                    Estado = "Completado"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al cargar formulario: " + ex.Message);
            }
        }

        // ✅ CREAR - POST
       [HttpPost]
public IActionResult Create(Pago pago)
{
    try
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdInscripcion", pago.IdInscripcion },
            { "@FechaPago", pago.FechaPago },
            { "@Monto", pago.Monto },
            { "@TipoPago", pago.TipoPago },
            { "@Estado", pago.Estado }
        };

        _db.EjecutarSPNonQuery("sp_InsertarPago", parametros);
        return Json(new { success = true });
    }
    catch (SqlException ex)
    {
        // ✅ Mensaje controlado
        if (ex.Message.Contains("Ya existe un pago registrado"))
            return Json(new { success = false, message = "⚠️ Esta inscripción ya tiene un pago registrado." });

        return Json(new { success = false, message = "Error al registrar el pago: " + ex.Message });
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
                var parametros = new Dictionary<string, object> { { "@IdPago", id } };
                var dt = _db.EjecutarSPDataTable("sp_ObtenerPagoPorId", parametros);

                if (dt.Rows.Count == 0)
                    return Content("No se encontró el pago.");

                var row = dt.Rows[0];
                var pago = new Pago
                {
                    IdPago = Convert.ToInt32(row["IdPago"]),
                    IdInscripcion = Convert.ToInt32(row["IdInscripcion"]),
                    FechaPago = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaPago"])),
                    Monto = Convert.ToDecimal(row["Monto"]),
                    TipoPago = row["TipoPago"].ToString(),
                    Estado = row["Estado"].ToString()
                };

                return PartialView("_EditPartial", pago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener los datos del pago: " + ex.Message);
            }
        }

        // ✅ EDITAR - POST
        [HttpPost]
        public IActionResult Edit(Pago pago)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdPago", pago.IdPago },
                    { "@IdInscripcion", pago.IdInscripcion },
                    { "@FechaPago", pago.FechaPago },
                    { "@Monto", pago.Monto },
                    { "@TipoPago", pago.TipoPago },
                    { "@Estado", pago.Estado }
                };

                _db.EjecutarSPNonQuery("sp_EditarPago", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = "Error SQL: " + ex.Message });
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
        // Enviar el IdPago a la vista parcial
        ViewBag.IdPago = id;
        return PartialView("_DeletePartial");
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error al cargar modal de eliminación: " + ex.Message);
    }
}


        // ✅ ELIMINAR - POST
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@IdPago", id } };
                _db.EjecutarSPNonQuery("sp_EliminarPago", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = "Error SQL: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error general: " + ex.Message });
            }
        }
    }
}
