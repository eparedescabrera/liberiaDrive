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
    public class CursosController : Controller
    {
        private readonly DatabaseService _db;

        public CursosController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // LISTAR CURSOS
        // =====================================================
      public IActionResult Index()
{
    var cursosDt = _db.EjecutarSPDataTable("sp_ListarCursos", null);

    // ✅ Evitar null si el SP no devuelve filas
    if (cursosDt == null || cursosDt.Rows.Count == 0)
        return View(new List<Curso>());

    // ✅ Convertir DataTable → List<Curso>
    var cursos = new List<Curso>();
    foreach (DataRow row in cursosDt.Rows)
    {
        cursos.Add(new Curso
        {
            IdCurso = Convert.ToInt32(row["IdCurso"]),
            TipoCurso = row["TipoCurso"].ToString()!,
            Duracion = Convert.ToInt32(row["Duracion"]),
            Costo = Convert.ToDecimal(row["Costo"])
        });
    }

    // ✅ Pasamos el modelo a la vista
    return View(cursos);
}

        // =====================================================
        // CREAR CURSO (GET)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View(new Curso());
        }

        // =====================================================
        // CREAR CURSO (POST)
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Curso model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var parametros = new Dictionary<string, object>
            {
                { "@TipoCurso", model.TipoCurso },
                { "@Costo", model.Costo },
                { "@Duracion", model.Duracion }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarCurso", parametros);
                TempData["Mensaje"] = "✅ Curso agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // =====================================================
        // EDITAR CURSO
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarCursos", null);
            var row = BuscarCursoPorId(dt, id);

            if (row == null)
                return NotFound();

            var curso = MapearCurso(row);
            return View(curso);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit([FromForm] Curso model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var parametros = new Dictionary<string, object>
            {
                { "@IdCurso", model.IdCurso },
                { "@TipoCurso", model.TipoCurso },
                { "@Costo", model.Costo },
                { "@Duracion", model.Duracion }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarCurso", parametros);
                TempData["Mensaje"] = "✏️ Curso actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // =====================================================
        // ELIMINAR CURSO
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarCursos", null);
            var row = BuscarCursoPorId(dt, id);

            if (row == null)
                return NotFound();

            var curso = MapearCurso(row);
            return View(curso);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdCurso)
        {
            var parametros = new Dictionary<string, object> { { "@IdCurso", IdCurso } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarCurso", parametros);
                TempData["Mensaje"] = "🗑️ Curso eliminado correctamente.";
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
        private static DataRow BuscarCursoPorId(DataTable dt, int id)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdCurso"]) == id)
                    return r;
            }
            return null;
        }
private static Curso MapearCurso(DataRow row)
{
    return new Curso
    {
        IdCurso = Convert.ToInt32(row["IdCurso"]),
        TipoCurso = row["TipoCurso"].ToString(),
        Costo = Convert.ToDecimal(row["Costo"]),
        Duracion = Convert.ToInt32(row["Duracion"])
    };
}
}
}