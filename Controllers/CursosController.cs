
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;


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

        // =======================
        // LISTAR
        // =======================
        public IActionResult Index()
        {
            var cursos = _db.EjecutarSPDataTable("sp_ListarCursos", null);
            var tiposCurso = _db.EjecutarSPDataTable("sp_ListarCursoTipo", null);

            ViewBag.TiposCurso = tiposCurso;
            ViewBag.Cursos = cursos;
            return View();
        }

        // =======================
        // CREAR (GET)
        // =======================
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            var tiposCurso = _db.EjecutarSPDataTable("sp_ListarCursoTipo", null);

            ViewBag.TiposCurso = tiposCurso.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["IdCursoTipo"].ToString(),
                    Text = row["NombreCursoTipo"].ToString()
                })
                .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CreatePartial", new Curso());

            return View(new Curso());
        }

        // =======================
        // CREAR (POST)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Curso model)
        {
            // Ignora validación del campo navegación
            ModelState.Remove("CursoTipo");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message = "Error en los datos: " + errores });
            }

            var parametros = new Dictionary<string, object>
            {
                { "@IdCursoTipo", model.IdCursoTipo },
                { "@Duracion", model.Duracion },
                { "@Costo", model.Costo }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarCurso", parametros);
                return Json(new { success = true, message = "Curso registrado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =======================
        // EDITAR (GET)
        // =======================
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdCurso", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerCursoPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("<div class='alert alert-warning text-center'>Curso no encontrado.</div>");

            var row = dt.Rows[0];
            var curso = new Curso
            {
                IdCurso = Convert.ToInt32(row["IdCurso"]),
                IdCursoTipo = Convert.ToInt32(row["IdCursoTipo"]),
                Duracion = Convert.ToInt32(row["Duracion"]),
                Costo = Convert.ToDecimal(row["Costo"])
            };

            var tiposCurso = _db.EjecutarSPDataTable("sp_ListarCursoTipo", null);
            ViewBag.TiposCurso = tiposCurso.AsEnumerable()
                .Select(r => new SelectListItem
                {
                    Value = r["IdCursoTipo"].ToString(),
                    Text = r["NombreCursoTipo"].ToString()
                })
                .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditPartial", curso);

            return View(curso);
        }

        // =======================
        // EDITAR (POST)
        // =======================
       [HttpPost]
[Authorize(Roles = "Administrador")]
public IActionResult Edit([FromForm] Curso model)
{
    // ✅ Ignorar validación del campo de navegación
    ModelState.Remove("CursoTipo");

    if (!ModelState.IsValid)
    {
        var errores = string.Join("; ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
        return Json(new { success = false, message = "Error en los datos: " + errores });
    }

    var parametros = new Dictionary<string, object>
    {
        { "@IdCurso", model.IdCurso },
        { "@IdCursoTipo", model.IdCursoTipo },
        { "@Duracion", model.Duracion },
        { "@Costo", model.Costo }
    };

    try
    {
        int filas = _db.EjecutarSPNonQuery("sp_ActualizarCurso", parametros);
        if (filas > 0)
            return Json(new { success = true, message = "Curso actualizado correctamente." });
        else
            return Json(new { success = false, message = "No se actualizó ningún registro." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}


        // =======================
        // DETALLES
        // =======================
        [HttpGet]
        public IActionResult Details(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdCurso", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerCursoPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("<div class='alert alert-warning text-center'>Curso no encontrado.</div>");

            var row = dt.Rows[0];
            var curso = new Curso
            {
                IdCurso = Convert.ToInt32(row["IdCurso"]),
                IdCursoTipo = Convert.ToInt32(row["IdCursoTipo"]),
                Duracion = Convert.ToInt32(row["Duracion"]),
                Costo = Convert.ToDecimal(row["Costo"])
            };

            ViewBag.CursoTipo = row["NombreCursoTipo"];

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DetailsPartial", curso);

            return View(curso);
        }

        // =======================
        // ELIMINAR
        // =======================
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdCurso", id } };
            var dt = _db.EjecutarSPDataTable("sp_ObtenerCursoPorId", parametros);

            if (dt.Rows.Count == 0)
                return Content("<div class='alert alert-warning text-center'>Curso no encontrado.</div>");

            var row = dt.Rows[0];
            var curso = new Curso
            {
                IdCurso = Convert.ToInt32(row["IdCurso"]),
                Duracion = Convert.ToInt32(row["Duracion"]),
                Costo = Convert.ToDecimal(row["Costo"])
            };

            ViewBag.CursoTipo = row["NombreCursoTipo"];

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DeletePartial", curso);

            return View(curso);
        }

        // =======================
        // ELIMINAR (POST)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdCurso)
        {
            var parametros = new Dictionary<string, object> { { "@IdCurso", IdCurso } };

            try
            {
                int filas = _db.EjecutarSPNonQuery("sp_EliminarCurso", parametros);

                if (filas > 0)
                    return Json(new { success = true, message = "Curso eliminado correctamente." });
                else
                    return Json(new { success = false, message = "No se pudo eliminar el curso (no existe o tiene dependencias)." });
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Json(new { success = false, message = "No se puede eliminar el curso porque tiene registros relacionados." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
