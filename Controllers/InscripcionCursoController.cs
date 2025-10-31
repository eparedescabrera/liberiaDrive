using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LiberiaDriveMVC.Controllers
{
    public class InscripcionCursoController : Controller
    {
        private readonly DatabaseService _db;

        public InscripcionCursoController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ LISTAR INSCRIPCIONES
        // =====================================================
        public IActionResult Index()
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ListarInscripciones", null);
                ViewBag.Inscripciones = dt;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "❌ Error al listar las inscripciones: " + ex.Message;
                return View();
            }
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
       [HttpGet]
public IActionResult Create()
{
    var model = new InscripcionCurso
    {
        FechaInscripcion = DateOnly.FromDateTime(DateTime.Today) // ✅ Fecha actual
    };

    return PartialView("_CreatePartial", model);
}

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
       [HttpPost]
public IActionResult Create(InscripcionCurso model)
{
    // 🔍 DEBUG: Ver qué datos llegan realmente
    Console.WriteLine($"DEBUG: IdCliente={model.IdCliente}, IdCurso={model.IdCurso}, Fecha={model.FechaInscripcion}");

    if (!ModelState.IsValid)
    {
        // 🔍 Muestra los errores exactos de validación
        var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        return Json(new { success = false, message = "Errores: " + string.Join(" | ", errores) });
    }

    try
    {
        var param = new Dictionary<string, object>
        {
            { "@IdCliente", model.IdCliente },
            { "@IdCurso", model.IdCurso },
            { "@FechaInscripcion", model.FechaInscripcion }
        };

        _db.EjecutarSPNonQuery("sp_RegistrarInscripcion", param);
        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
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
        var dt = _db.EjecutarSPDataTable("sp_ObtenerInscripcionPorId",
            new Dictionary<string, object> { { "@IdInscripcion", id } });

        if (dt.Rows.Count == 0)
            return Content("<div class='alert alert-warning'>No se encontró la inscripción.</div>");

        var row = dt.Rows[0];
        var inscripcion = new InscripcionCurso
        {
            IdInscripcion = id,
            IdCliente = Convert.ToInt32(row["IdCliente"]),
            IdCurso = Convert.ToInt32(row["IdCurso"]),
            FechaInscripcion = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaInscripcion"]))
        };

        ViewBag.NombreCliente = row["NombreCliente"].ToString();
        ViewBag.TipoCurso = row["TipoCurso"].ToString();

        return PartialView("_EditPartial", inscripcion);
    }
    catch (Exception ex)
    {
        return Content($"<div class='text-danger'>❌ Error: {ex.Message}</div>");
    }
}


        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
       [HttpPost]
public IActionResult Edit(InscripcionCurso model)
{
    if (!ModelState.IsValid)
        return Json(new { success = false, message = "Datos inválidos del formulario." });

    try
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdInscripcion", model.IdInscripcion },
            { "@IdCliente", model.IdCliente },
            { "@IdCurso", model.IdCurso },
            { "@FechaInscripcion", model.FechaInscripcion }
        };

        _db.EjecutarSPNonQuery("sp_ActualizarInscripcion", parametros);
        return Json(new { success = true, message = "Inscripción actualizada correctamente." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}


        // =====================================================
        // ✅ ELIMINAR - GET
        // =====================================================
       [HttpGet]
public IActionResult Delete(int id)
{
    var dt = _db.EjecutarSPDataTable("sp_ObtenerInscripcionPorId",
        new Dictionary<string, object> { { "@IdInscripcion", id } });

    if (dt.Rows.Count == 0)
        return Content("<div class='alert alert-warning text-center'>Inscripción no encontrada.</div>");

    var row = dt.Rows[0];

    var inscripcion = new
    {
        IdInscripcion = id,
        NombreCliente = row["NombreCliente"].ToString(),
        TipoCurso = row["TipoCurso"].ToString(),
        FechaInscripcion = Convert.ToDateTime(row["FechaInscripcion"]).ToString("dd/MM/yyyy")
    };

    return PartialView("_DeletePartial", inscripcion);
}
        // =====================================================
        // ✅ ELIMINAR - POST
        // =====================================================
       [HttpPost]
public IActionResult DeleteConfirmed(int IdInscripcion)
{
    try
    {
        _db.EjecutarSPNonQuery("sp_EliminarInscripcion",
            new Dictionary<string, object> { { "@IdInscripcion", IdInscripcion } });

        return Json(new { success = true, message = "Inscripción eliminada correctamente." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}
        // =====================================================
        // 🔍 BUSCADOR AJAX DE CLIENTES (para Select2)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarClientes(string term)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@Texto", term ?? "" }
            };

            var dt = _db.EjecutarSPDataTable("sp_BuscarClientes", parametros);

            var resultados = dt.AsEnumerable().Select(r => new
            {
                id = r["IdCliente"],
                text = r["NombreCompleto"].ToString()
            });

            return Json(resultados);
        }

        // =====================================================
        // 🔍 BUSCADOR AJAX DE CURSOS (para Select2)
        // =====================================================
        [HttpGet]
        [HttpGet]
public IActionResult BuscarCursos(string term)
{
    var parametros = new Dictionary<string, object> { { "@Texto", term ?? "" } };
    var dt = _db.EjecutarSPDataTable("sp_BuscarCursos", parametros);

    var resultados = dt.AsEnumerable().Select(r => new
    {
        id = r["IdCurso"],
        text = r["CursoDescripcion"].ToString()
    });

    return Json(resultados);
}

    }
}
