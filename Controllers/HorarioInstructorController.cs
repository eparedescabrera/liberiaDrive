using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace LiberiaDriveMVC.Controllers
{
    public class HorarioInstructorController : Controller
    {
        private readonly DatabaseService _db;

        public HorarioInstructorController(DatabaseService db)
        {
            _db = db;
        }

        // ==============================================
        // ✅ INDEX - LISTAR HORARIOS
        // ==============================================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarHorariosInstructor", null);
            ViewBag.Horarios = dt;
            return View();
        }

        // ==============================================
        // ✅ CREAR - GET
        // ==============================================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new HorarioInstructor());
        }

        // ==============================================
        // ✅ CREAR - POST
        // ==============================================
        [HttpPost]
        public IActionResult Create(HorarioInstructor model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdInstructor", model.IdInstructor },
                    { "@DiaSemana", model.DiaSemana },
                    { "@HoraInicio", model.HoraInicio },
                    { "@HoraFin", model.HoraFin },
                    { "@Disponible", model.Disponible }
                };

                _db.EjecutarSPNonQuery("sp_InsertarHorarioInstructor", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==============================================
        // ✅ EDITAR - GET
        // ==============================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdHorario", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerHorarioInstructorPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("No se encontró el horario.");

            var row = dt.Rows[0];
            var model = new HorarioInstructor
            {
                IdHorario = Convert.ToInt32(row["IdHorario"]),
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                DiaSemana = row["DiaSemana"].ToString(),
                HoraInicio = (TimeSpan)row["HoraInicio"],
                HoraFin = (TimeSpan)row["HoraFin"],
                Disponible = Convert.ToBoolean(row["Disponible"]),
                IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() }
            };

            return PartialView("_EditPartial", model);
        }

        // ==============================================
        // ✅ EDITAR - POST
        // ==============================================
        [HttpPost]
        public IActionResult Edit(HorarioInstructor model)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@IdHorario", model.IdHorario },
                    { "@IdInstructor", model.IdInstructor },
                    { "@DiaSemana", model.DiaSemana },
                    { "@HoraInicio", model.HoraInicio },
                    { "@HoraFin", model.HoraFin },
                    { "@Disponible", model.Disponible }
                };

                _db.EjecutarSPNonQuery("sp_ActualizarHorarioInstructor", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==============================================
        // ✅ ELIMINAR - GET
        // ==============================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdHorario", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerHorarioInstructorPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("No se encontró el horario.");

            var row = dt.Rows[0];
            var model = new HorarioInstructor
            {
                IdHorario = Convert.ToInt32(row["IdHorario"]),
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                DiaSemana = row["DiaSemana"].ToString(),
                HoraInicio = (TimeSpan)row["HoraInicio"],
                HoraFin = (TimeSpan)row["HoraFin"],
                Disponible = Convert.ToBoolean(row["Disponible"]),
                IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() }
            };

            return PartialView("_DeletePartial", model);
        }

        // ==============================================
        // ✅ ELIMINAR - POST
        // ==============================================
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@IdHorario", id } };
                _db.EjecutarSPNonQuery("sp_EliminarHorarioInstructor", parametros);
                return Json(new { success = true });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==============================================
        // 🔍 BUSCAR INSTRUCTORES ACTIVOS
        // ==============================================
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
    }
}
