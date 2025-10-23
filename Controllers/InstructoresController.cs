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
    public class InstructoresController : Controller
    {
        private readonly DatabaseService _db;

        public InstructoresController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // LISTAR INSTRUCTORES
        // =====================================================
        public IActionResult Index()
        {
            var instructores = _db.EjecutarSPDataTable("sp_ListarInstructores", null);
            ViewBag.Instructores = instructores;
            return View();
        }

        // =====================================================
        // CREAR INSTRUCTOR - GET
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var instructor = new Instructor { Estado = true };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CreatePartial", instructor);

            return View(instructor);
        }

        // =====================================================
        // CREAR INSTRUCTOR - POST
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Instructor model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreatePartial", model);

            var parametros = new Dictionary<string, object>
            {
                { "@Nombre", model.Nombre.Trim() },
                { "@Apellidos", model.Apellidos.Trim() },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarInstructor", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // EDITAR INSTRUCTOR - GET
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarInstructores", null);

            DataRow row = null;
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdInstructor"]) == id)
                {
                    row = r;
                    break;
                }
            }

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Instructor no encontrado.</div>", "text/html");

            var instructor = new Instructor
            {
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditPartial", instructor);

            return View(instructor);
        }

        // =====================================================
        // EDITAR INSTRUCTOR - POST
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit([FromForm] Instructor model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditPartial", model);

            var parametros = new Dictionary<string, object>
            {
                { "@IdInstructor", model.IdInstructor },
                { "@Nombre", model.Nombre.Trim() },
                { "@Apellidos", model.Apellidos.Trim() },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarInstructor", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // DETALLES - GET
        // =====================================================
        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Details(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarInstructores", null);

            DataRow row = null;
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdInstructor"]) == id)
                {
                    row = r;
                    break;
                }
            }

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Instructor no encontrado.</div>", "text/html");

            var instructor = new Instructor
            {
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DetailsPartial", instructor);

            return View(instructor);
        }

        // =====================================================
        // ELIMINAR INSTRUCTOR - GET (muestra modal)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarInstructores", null);

            DataRow row = null;
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdInstructor"]) == id)
                {
                    row = r;
                    break;
                }
            }

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Instructor no encontrado.</div>", "text/html");

            var instructor = new Instructor
            {
                IdInstructor = Convert.ToInt32(row["IdInstructor"]),
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DeletePartial", instructor);

            return View(instructor);
        }

        // =====================================================
        // ELIMINAR INSTRUCTOR - POST CONFIRMADO
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdInstructor)
        {
            var parametros = new Dictionary<string, object> { { "@IdInstructor", IdInstructor } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarInstructor", parametros);
                return Json(new { success = true, message = "Instructor eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
