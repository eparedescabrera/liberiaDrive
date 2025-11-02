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
    public class ResultadoExamenController : Controller
    {
        private readonly DatabaseService _db;
        public ResultadoExamenController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // ✅ INDEX - LISTAR RESULTADOS
        // =====================================================
        public IActionResult Index()
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarResultados", null);
            ViewBag.Resultados = dt;
            return View();
        }

        // =====================================================
        // ✅ CREAR - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new ResultadoExamen
            {
                FechaExamen = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        // =====================================================
        // ✅ CREAR - POST
        // =====================================================
       [HttpPost]
public IActionResult Create(ResultadoExamen model)
{
    try
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdCliente", model.IdCliente },
            { "@TipoExamen", model.TipoExamen },
            { "@FechaExamen", model.FechaExamen },
            { "@Aprobado", model.Aprobado },
            { "@IdInstructor", model.IdInstructor }
        };

        _db.EjecutarSPNonQuery("sp_CrearResultadoExamen", parametros);

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
        var parametros = new Dictionary<string, object>
        {
            { "@IdResultado", id }
        };

        var dt = _db.EjecutarSPDataTable("sp_ObtenerResultadoPorId", parametros);
        if (dt.Rows.Count == 0)
            return Content("No se encontró el resultado.");

        var row = dt.Rows[0];
        var model = new ResultadoExamen
        {
            IdResultado = Convert.ToInt32(row["IdResultado"]),
            IdCliente = Convert.ToInt32(row["IdCliente"]),
            TipoExamen = row["TipoExamen"].ToString(),
            FechaExamen = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaExamen"])),
            Aprobado = Convert.ToBoolean(row["Aprobado"]),
            IdInstructor = row["IdInstructor"] == DBNull.Value ? null : Convert.ToInt32(row["IdInstructor"]),
            IdClienteNavigation = new Cliente { Nombre = row["NombreCliente"].ToString() },
            IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() }
        };

        return PartialView("_EditPartial", model);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}



        // =====================================================
        // ✅ EDITAR - POST
        // =====================================================
       [HttpPost]
public JsonResult Edit(ResultadoExamen model)
{
    try
    {
       var fecha = model.FechaExamen == default ? DateOnly.FromDateTime(DateTime.Now) : model.FechaExamen;

var parametros = new Dictionary<string, object>
{
    { "@IdResultado", model.IdResultado },
    { "@IdCliente", model.IdCliente },
    { "@TipoExamen", model.TipoExamen },
    { "@FechaExamen", fecha.ToDateTime(TimeOnly.MinValue) },  // ✅ convertir DateOnly a DateTime
    { "@Aprobado", model.Aprobado },
    { "@IdInstructor", (object?)model.IdInstructor ?? DBNull.Value }
};


        _db.EjecutarSPNonQuery("sp_ActualizarResultado", parametros);

        return Json(new { success = true });
    }
    catch (SqlException ex)
    {
        // Si viene del RAISERROR, devolver mensaje amigable
        if (ex.Number == 50000 || ex.Message.Contains("❌"))
        {
            return Json(new { success = false, message = ex.Message });
        }

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
        var parametros = new Dictionary<string, object>
        {
            { "@IdResultado", id }
        };

        var dt = _db.EjecutarSPDataTable("sp_ObtenerResultadoPorId", parametros);
        if (dt.Rows.Count == 0)
            return Content("No se encontró el resultado.");

        var row = dt.Rows[0];
        var model = new ResultadoExamen
        {
            IdResultado = Convert.ToInt32(row["IdResultado"]),
            IdCliente = Convert.ToInt32(row["IdCliente"]),
            TipoExamen = row["TipoExamen"].ToString(),
            FechaExamen = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaExamen"])),
            IdInstructor = row["IdInstructor"] == DBNull.Value ? null : Convert.ToInt32(row["IdInstructor"]),
            IdClienteNavigation = new Cliente { Nombre = row["NombreCliente"].ToString() },
            IdInstructorNavigation = new Instructor { Nombre = row["NombreInstructor"].ToString() }
        };

        return PartialView("_DeletePartial", model);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al cargar datos: {ex.Message}");
    }
}


        // =====================================================
        // ✅ ELIMINAR - POST
        // =====================================================
       [HttpPost]
public IActionResult DeleteConfirmed(int id)
{
    try
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdResultado", id }
        };

        _db.EjecutarSPNonQuery("sp_EliminarResultadoExamen", parametros);
        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

        // =====================================================
        // 🔍 BUSCADOR DE CLIENTES E INSTRUCTORES (AJAX)
        // =====================================================
        [HttpGet]
        public IActionResult BuscarClientes(string term)
        {
            var parametros = new Dictionary<string, object> { { "@Texto", term ?? "" } };
            var dt = _db.EjecutarSPDataTable("sp_BuscarClientes", parametros);

            var resultados = dt.AsEnumerable().Select(r => new
            {
                id = r["IdCliente"],
                text = r["NombreCompleto"].ToString()
            });

            return Json(resultados);
        }

       // =====================================================
// 🔍 BUSCADOR AJAX DE INSTRUCTORES
// =====================================================
[HttpGet]
public IActionResult BuscarInstructores(string term)
{
    var parametros = new Dictionary<string, object>
    {
        { "@Texto", term ?? "" }
    };

    // Usa un SP que filtre por nombre/apellido del instructor
    var dt = _db.EjecutarSPDataTable("sp_BuscarInstructores", parametros);

    var resultados = dt.AsEnumerable().Select(r => new
    {
        id = r["IdInstructor"],
        text = r["NombreCompleto"].ToString()
    });

    return Json(resultados);
}

    }
}
