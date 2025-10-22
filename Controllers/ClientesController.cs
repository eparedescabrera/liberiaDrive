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
            var clientes = _db.EjecutarSPDataTable("sp_ListarClientes", null);
            ViewBag.Clientes = clientes;
            return View();
        }

        // =====================================================
        // CREAR CLIENTE
        // =====================================================
        [Authorize(Roles = "Administrador")]
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

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Cliente model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreatePartial", model);

            var parametros = new Dictionary<string, object>
            {
                { "@Nombre", model.Nombre.Trim() },
                { "@Apellidos", model.Apellidos.Trim() },
                { "@Contacto", model.Contacto ?? (object)DBNull.Value },
                { "@FechaRegistro", model.FechaRegistro.ToDateTime(TimeOnly.MinValue) },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarCliente", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // EDITAR CLIENTE - GET (trae datos)
        // =====================================================
       [Authorize(Roles = "Administrador,Instructor")]
public IActionResult Edit(int id)
{
    try
    {
        // 1️⃣ Ejecutamos sp_ListarClientes para traer todos
        var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);

        // 2️⃣ Buscamos solo el cliente con el Id solicitado
        DataRow row = null;
        foreach (DataRow r in dt.Rows)
        {
            if (Convert.ToInt32(r["IdCliente"]) == id)
            {
                row = r;
                break;
            }
        }

        // 3️⃣ Si no lo encontró, devolvemos mensaje visible
        if (row == null)
            return Content("<div class='alert alert-warning text-center'>⚠️ Cliente no encontrado.</div>", "text/html");

        // 4️⃣ Creamos el objeto Cliente con los datos
        var cliente = new Cliente
        {
            IdCliente = Convert.ToInt32(row["IdCliente"]),
            Nombre = row["Nombre"].ToString(),
            Apellidos = row["Apellidos"].ToString(),
            Contacto = row["Contacto"] == DBNull.Value ? "" : row["Contacto"].ToString(),
            FechaRegistro = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaRegistro"])),
            Estado = Convert.ToBoolean(row["Estado"])
        };

        // 5️⃣ Retornamos el partial al modal si fue una petición AJAX
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_EditPartial", cliente);

        // 6️⃣ O mostramos la vista normal (raro, pero por seguridad)
        return View(cliente);
    }
    catch (Exception ex)
    {
        // 7️⃣ Si hubo error SQL o conexión
        return Content($"<div class='alert alert-danger text-center'>❌ Error al cargar los datos:<br>{ex.Message}</div>", "text/html");
    }
}

        // =====================================================
        // EDITAR CLIENTE - POST (usa sp_ActualizarCliente)
        // =====================================================
       [HttpPost]
[Authorize(Roles = "Administrador")]
public IActionResult Edit([FromForm] Cliente model)
{
    if (!ModelState.IsValid)
        return PartialView("_EditPartial", model);

    var parametros = new Dictionary<string, object>
    {
        { "@IdCliente", model.IdCliente },
        { "@Nombre", model.Nombre.Trim() },
        { "@Apellidos", model.Apellidos.Trim() },
        { "@Contacto", model.Contacto ?? (object)DBNull.Value },
        { "@FechaRegistro", model.FechaRegistro.ToDateTime(TimeOnly.MinValue) },
        { "@Estado", model.Estado }
    };

    try
    {
        _db.EjecutarSPNonQuery("sp_ActualizarCliente", parametros);
        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}
// =====================================================
// DETALLES CLIENTE - GET (usa sp_ListarClientes)
// =====================================================
[Authorize(Roles = "Administrador,Instructor")]
public IActionResult Details(int id)
{
    try
    {
        // 1️⃣ Obtenemos todos los clientes desde el procedimiento
        var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);

        // 2️⃣ Buscamos el cliente con el Id solicitado
        DataRow row = null;
        foreach (DataRow r in dt.Rows)
        {
            if (Convert.ToInt32(r["IdCliente"]) == id)
            {
                row = r;
                break;
            }
        }

        // 3️⃣ Si no lo encontró, devolvemos mensaje visible
        if (row == null)
            return Content("<div class='alert alert-warning text-center'>⚠️ Cliente no encontrado.</div>", "text/html");

        // 4️⃣ Creamos el objeto Cliente
        var cliente = new Cliente
        {
            IdCliente = Convert.ToInt32(row["IdCliente"]),
            Nombre = row["Nombre"].ToString(),
            Apellidos = row["Apellidos"].ToString(),
            Contacto = row["Contacto"] == DBNull.Value ? "" : row["Contacto"].ToString(),
            FechaRegistro = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaRegistro"])),
            Estado = Convert.ToBoolean(row["Estado"])
        };

        // 5️⃣ Si es una solicitud AJAX, devolvemos el partial
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_DetailsPartial", cliente);

        // 6️⃣ Si se accede directamente (poco común), mostramos vista completa
        return View(cliente);
    }
    catch (Exception ex)
    {
        // 7️⃣ Si ocurre un error SQL o de conexión, lo mostramos en el modal
        return Content($"<div class='alert alert-danger text-center'>❌ Error al cargar detalles:<br>{ex.Message}</div>", "text/html");
    }
}

        // =====================================================
        // ELIMINAR CLIENTE
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdCliente", id } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarCliente", parametros);
                TempData["Success"] = "🗑️ Cliente eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
