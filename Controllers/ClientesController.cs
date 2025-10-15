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
    public class ClientesController : Controller
    {
        private readonly DatabaseService _db;

        public ClientesController(DatabaseService db)
        {
            _db = db;
        }

        // =====================================================
        // LISTAR CLIENTES
        // =====================================================
        public IActionResult Index()
        {
            var clientes = _db.EjecutarSPDataTable("SELECT * FROM Cliente ORDER BY IdCliente DESC", null, true);
            ViewBag.Clientes = clientes;
            return View();
        }

        // =====================================================
        // CREAR CLIENTE - GET
        // =====================================================
        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Create()
        {
            var cliente = new Cliente
            {
                FechaRegistro = DateOnly.FromDateTime(DateTime.Now),
                Estado = true
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CreatePartial", cliente);

            return View(cliente);
        }

        // =====================================================
        // CREAR CLIENTE - POST
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Create([FromForm] Cliente model)
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
                { "@Contacto", string.IsNullOrWhiteSpace(model.Contacto) ? (object)DBNull.Value : model.Contacto.Trim() },
                { "@FechaRegistro", model.FechaRegistro.ToDateTime(TimeOnly.MinValue) },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarCliente", parametros);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });

                TempData["Success"] = "✅ Cliente registrado correctamente.";
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

        
// DETALLES CLIENTE - GET
// =====================================================
public IActionResult Details(int id)
{
    string sql = "SELECT * FROM Cliente WHERE IdCliente = @IdCliente";
    var parametros = new Dictionary<string, object> { { "@IdCliente", id } };
    var dt = _db.EjecutarSPDataTable(sql, parametros);

    if (dt.Rows.Count == 0)
        return NotFound();

    var row = dt.Rows[0];
    var cliente = new Cliente
    {
        IdCliente = Convert.ToInt32(row["IdCliente"]),
        Nombre = row["Nombre"].ToString(),
        Apellidos = row["Apellidos"].ToString(),
        Contacto = row["Contacto"] == DBNull.Value ? "" : row["Contacto"].ToString(),
        FechaRegistro = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaRegistro"])),
        Estado = Convert.ToBoolean(row["Estado"])
    };

    // ✅ Si viene del modal AJAX
    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        return PartialView("_DetailsPartial", cliente);

    // ✅ Si entra directo (por URL)
    return View(cliente);
}

        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Edit(int id)
        {
            string sql = "SELECT * FROM Cliente WHERE IdCliente = @IdCliente";
            var parametros = new Dictionary<string, object> { { "@IdCliente", id } };
            var dt = _db.EjecutarSPDataTable(sql, parametros);

            if (dt.Rows.Count == 0)
                return NotFound();

            var row = dt.Rows[0];
            var cliente = new Cliente
            {
                IdCliente = Convert.ToInt32(row["IdCliente"]),
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Contacto = row["Contacto"] == DBNull.Value ? "" : row["Contacto"].ToString(),
                FechaRegistro = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaRegistro"])),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditPartial", cliente);

            return View(cliente);
        }

        // =====================================================
        // EDITAR CLIENTE - POST
        // =====================================================
        [HttpPost]
[Authorize(Roles = "Administrador,Instructor")]
public IActionResult Edit([FromForm] Cliente model)
{
    if (!ModelState.IsValid)
    {
        // Si hay errores de validación, los devolvemos al modal
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_EditPartial", model);

        return View(model);
    }

    var parametros = new Dictionary<string, object>
    {
        { "@IdCliente", model.IdCliente },
        { "@Nombre", model.Nombre.Trim() },
        { "@Apellidos", model.Apellidos.Trim() },
        { "@Contacto", string.IsNullOrWhiteSpace(model.Contacto) ? (object)DBNull.Value : model.Contacto.Trim() },
        { "@FechaRegistro", model.FechaRegistro.ToDateTime(TimeOnly.MinValue) },
        { "@Estado", model.Estado }
    };

    try
    {
        _db.EjecutarSPNonQuery("sp_ActualizarCliente", parametros);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true });

        TempData["Success"] = "✅ Cliente actualizado correctamente.";
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

        // =====================================================
        // ELIMINAR CLIENTE
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdCliente", id } };
            _db.EjecutarSPNonQuery("sp_EliminarCliente", parametros);

            TempData["Success"] = "🗑️ Cliente eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
