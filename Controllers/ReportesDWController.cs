using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    public class ReportesDWController : Controller
    {
        private readonly DatabaseService _db;

        public ReportesDWController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ VISTA PRINCIPAL DEL PANEL DE REPORTES
        // =====================================================
        public IActionResult Index()
        {
            try
            {
                // Obtener los datos de resumen iniciales desde el DW
                var resumenInstructor = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorInstructorDW");
                var resumenEstado = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorEstadoDW");
                var resumenMes = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorMesDW");

                ViewBag.ResumenInstructor = resumenInstructor;
                ViewBag.ResumenEstado = resumenEstado;
                ViewBag.ResumenMes = resumenMes;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los reportes: " + ex.Message;
                return View();
            }
        }

        // =====================================================
        // 🔁 ACTUALIZAR EL DATA WAREHOUSE (Botón “Actualizar DW”)
        // =====================================================
        [HttpPost]
        public IActionResult ActualizarDW()
        {
            try
            {
                _db.EjecutarSPNonQueryDW("sp_Actualizar_SesionesDW");
                return Json(new { success = true, message = "✅ El Data Warehouse fue actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error al actualizar el DW: " + ex.Message });
            }
        }

        // =====================================================
        // 📊 ENDPOINTS PARA GRÁFICAS (AJAX)
        // =====================================================

        [HttpGet]
        public IActionResult ObtenerResumenInstructor()
        {
            var dt = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorInstructorDW");
            var datos = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                datos.Add(new
                {
                    Instructor = row["Instructor"].ToString(),
                    Total = Convert.ToInt32(row["TotalSesiones"])
                });
            }

            return Json(datos);
        }

        [HttpGet]
        public IActionResult ObtenerResumenEstado()
        {
            var dt = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorEstadoDW");
            var datos = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                datos.Add(new
                {
                    Estado = row["Estado"].ToString(),
                    Total = Convert.ToInt32(row["TotalSesiones"])
                });
            }

            return Json(datos);
        }

        [HttpGet]
        public IActionResult ObtenerResumenMes()
        {
            var dt = _db.EjecutarSPDataTableDW("sp_ResumenSesionesPorMesDW");
            var datos = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                datos.Add(new
                {
                    Mes = row["Mes"].ToString(),
                    Total = Convert.ToInt32(row["TotalSesiones"])
                });
            }

            return Json(datos);
        }
    }
}
