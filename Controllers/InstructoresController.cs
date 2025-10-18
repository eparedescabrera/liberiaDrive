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

        // =======================
        // LISTAR
        // =======================
        public IActionResult Index()
        {
            var instructores = _db.EjecutarSPDataTable("SELECT * FROM Instructor ORDER BY IdInstructor DESC", null, true);
            ViewBag.Instructores = instructores;
            return View();
        }

        // =======================
        // CREAR (GET)
        // =======================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var instructor = new Instructor { Estado = true };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CreatePartial", instructor);

            return View(instructor);
        }

        // =======================
        // CREAR (POST)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Instructor model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_CreatePartial", model);

                return View(model);
            }

            var parametros = new Dictionary<string, object>
            {
                { "@Nombre", model.Nombre.Trim() },
                { "@Apellidos", model.Apellidos.Trim() },
                { "@Estado", model.Estado ?? true }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarInstructor", parametros);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });

                TempData["Success"] = "✅ Instructor registrado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = ex.Message });

                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // =======================
        // EDITAR (GET)
        // =======================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdInstructor", id } };
            var dt = _db.EjecutarSPDataTable("SELECT * FROM Instructor WHERE IdInstructor = @IdInstructor", parametros);

            if (dt.Rows.Count == 0)
                return NotFound();

            var row = dt.Rows[0];
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

        // =======================
        // EDITAR (POST)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit([FromForm] Instructor model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_EditPartial", model);

                return View(model);
            }

            var parametros = new Dictionary<string, object>
            {
                { "@IdInstructor", model.IdInstructor },
                { "@Nombre", model.Nombre.Trim() },
                { "@Apellidos", model.Apellidos.Trim() },
                { "@Estado", model.Estado ?? true }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarInstructor", parametros);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });

                TempData["Success"] = "✅ Instructor actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = ex.Message });

                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // =======================
        // DETALLES
        // =======================
        public IActionResult Details(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdInstructor", id } };
            var dt = _db.EjecutarSPDataTable("SELECT * FROM Instructor WHERE IdInstructor = @IdInstructor", parametros);

            if (dt.Rows.Count == 0)
                return NotFound();

            ViewBag.Instructor = dt.Rows[0];

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DetailsPartial");

            return View();
        }

        // =======================
        // ELIMINAR
        // =======================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdInstructor", id } };
            _db.EjecutarSPNonQuery("sp_EliminarInstructor", parametros);

            TempData["Success"] = "🗑️ Instructor eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
