using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        // ✅ LISTAR INSTRUCTORES
        // =====================================================
       public IActionResult Index()
{
    var instructores = _db.EjecutarSPDataTable("sp_ListarInstructores", null);

    // ✅ Traer licencias por instructor
    var licDt = _db.EjecutarSPDataTable("sp_ListarLicenciasInstructor", null);

    // ✅ Convertir a estructura fácil de usar en la vista
    var licPorInstructor = new Dictionary<int, List<string>>();

    foreach (DataRow row in licDt.Rows)
    {
        int id = Convert.ToInt32(row["IdInstructor"]);
        string licencia = row["TipoLicencia"].ToString();

        if (!licPorInstructor.ContainsKey(id))
            licPorInstructor[id] = new List<string>();

        licPorInstructor[id].Add(licencia);
    }

    ViewBag.Instructores = instructores;
    ViewBag.LicenciasPorInstructor = licPorInstructor;

    return View();
}

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            CargarLicencias();
            return PartialView("_CreatePartial", new Instructor());
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Create(Instructor model, int[] licenciasSeleccionadas)
        {
            if (!ModelState.IsValid)
            {
                CargarLicencias();
                return PartialView("_CreatePartial", model);
            }

            var parametros = new Dictionary<string, object>
            {
                { "@Nombre", model.Nombre },
                { "@Apellidos", model.Apellidos },
                { "@Estado", model.Estado ?? true }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarInstructor", parametros);

                // Obtener ID del último instructor insertado
                var dt = _db.EjecutarSPDataTable("SELECT TOP 1 IdInstructor FROM Instructor ORDER BY IdInstructor DESC", null, true);
                int idInstructor = Convert.ToInt32(dt.Rows[0]["IdInstructor"]);

                // Insertar licencias seleccionadas
                foreach (var idLic in licenciasSeleccionadas)
                {
                    var paramLic = new Dictionary<string, object>
                    {
                        { "@IdInstructor", idInstructor },
                        { "@IdLicencia", idLic }
                    };

                    _db.EjecutarSPNonQuery("sp_InsertarInstructorLicencia", paramLic);
                }

                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                CargarLicencias();
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ EDITAR - GET
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ObtenerInstructorPorId",
                new Dictionary<string, object> { { "@IdInstructor", id } });

            if (dt.Rows.Count == 0)
                return Content("<div class='alert alert-warning text-center'>Instructor no encontrado.</div>");

            var row = dt.Rows[0];

            var instructor = new Instructor
            {
                IdInstructor = id,
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            // Licencias seleccionadas
            var dtLic = _db.EjecutarSPDataTable("sp_ListarLicenciasPorInstructor",
                new Dictionary<string, object> { { "@IdInstructor", id } });

            ViewBag.LicenciasInstructor = dtLic.AsEnumerable()
                .Select(r => Convert.ToInt32(r["IdLicencia"])).ToList();

            CargarLicencias();

            return PartialView("_EditPartial", instructor);
        }

        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Edit(Instructor model, int[] licenciasSeleccionadas)
        {
            if (!ModelState.IsValid)
            {
                CargarLicencias();
                return PartialView("_EditPartial", model);
            }

            var parametros = new Dictionary<string, object>
            {
                { "@IdInstructor", model.IdInstructor },
                { "@Nombre", model.Nombre },
                { "@Apellidos", model.Apellidos },
                { "@Estado", model.Estado ?? true }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarInstructor", parametros);

                // Eliminar licencias previas
                _db.EjecutarSPNonQuery("sp_EliminarInstructorLicencias",
                    new Dictionary<string, object> { { "@IdInstructor", model.IdInstructor } });

                // Insertar nuevas licencias seleccionadas
                foreach (var idLic in licenciasSeleccionadas)
                {
                    var parametrosLic = new Dictionary<string, object>
                    {
                        { "@IdInstructor", model.IdInstructor },
                        { "@IdLicencia", idLic }
                    };

                    _db.EjecutarSPNonQuery("sp_InsertarInstructorLicencia", parametrosLic);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                CargarLicencias();
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ✅ DETALLES - GET
        // =====================================================
        public IActionResult Details(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ObtenerInstructorPorId",
                new Dictionary<string, object> { { "@IdInstructor", id } });

            if (dt.Rows.Count == 0)
                return Content("Instructor no encontrado");

            var row = dt.Rows[0];

            var instructor = new Instructor
            {
                IdInstructor = id,
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            var dtLic = _db.EjecutarSPDataTable("sp_ListarLicenciasPorInstructor",
                new Dictionary<string, object> { { "@IdInstructor", id } });

            ViewBag.LicenciasInstructor = dtLic.AsEnumerable()
                .Select(r => r["TipoLicencia"].ToString()).ToList();

            return PartialView("_DetailsPartial", instructor);
        }

        // =====================================================
        // ✅ ELIMINAR - POST
        // =====================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult DeleteConfirmed(int IdInstructor)
        {
            try
            {
                // Eliminar relación con licencias
                _db.EjecutarSPNonQuery("sp_EliminarInstructorLicencias",
                    new Dictionary<string, object> { { "@IdInstructor", IdInstructor } });

                // Eliminar instructor
                _db.EjecutarSPNonQuery("sp_EliminarInstructor",
                    new Dictionary<string, object> { { "@IdInstructor", IdInstructor } });

                return Json(new { success = true, message = "Instructor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
// =====================================================
// ✅ ELIMINAR - GET
// =====================================================
[Authorize(Roles = "Administrador")]
[HttpGet]
public IActionResult Delete(int id)
{
    var dt = _db.EjecutarSPDataTable("sp_ObtenerInstructorPorId",
        new Dictionary<string, object> { { "@IdInstructor", id } });

    if (dt.Rows.Count == 0)
        return Content("<div class='alert alert-warning text-center'>Instructor no encontrado.</div>");

    var row = dt.Rows[0];

    var instructor = new Instructor
    {
        IdInstructor = id,
        Nombre = row["Nombre"].ToString(),
        Apellidos = row["Apellidos"].ToString(),
        Estado = Convert.ToBoolean(row["Estado"])
    };

    return PartialView("_DeletePartial", instructor);
}

        // =====================================================
        // *Cargar Licencias en Create y Edit
        // =====================================================
        private void CargarLicencias()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarLicencias", null);

            var lista = dt.AsEnumerable().Select(r => new Licencia
            {
                IdLicencia = Convert.ToInt32(r["IdLicencia"]),
                TipoLicencia = r["TipoLicencia"].ToString()
            }).ToList();

            ViewBag.Licencias = lista;
        }
    }
}
