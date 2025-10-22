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
        // LISTAR CLIENTES (usa SP)
        // =====================================================
        public IActionResult Index()
        {
            var clientes = _db.EjecutarSPDataTable("sp_ListarClientes", null);
            ViewBag.Clientes = clientes;
            return View();
        }

        // =====================================================
        // CREAR CLIENTE (GET)
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

        // =====================================================
        // CREAR CLIENTE (POST) - usa SP
        // =====================================================
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
                return Json(new { success = true, message = "Cliente agregado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // =====================================================
        // EDITAR CLIENTE - GET (usa SP)
        // =====================================================
        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Edit(int id)
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);
                var row = BuscarClientePorId(dt, id);

                if (row == null)
                    return Content("<div class='alert alert-warning text-center'>⚠️ Cliente no encontrado.</div>", "text/html");

                var cliente = MapearCliente(row);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_EditPartial", cliente);

                return View(cliente);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger text-center'>❌ Error al cargar los datos:<br>{ex.Message}</div>", "text/html");
            }
        }

        // =====================================================
        // EDITAR CLIENTE - POST (usa SP)
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
                return Json(new { success = true, message = "Cliente actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // =====================================================
        // DETALLES CLIENTE (usa SP)
        // =====================================================
        [Authorize(Roles = "Administrador,Instructor")]
        public IActionResult Details(int id)
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);
                var row = BuscarClientePorId(dt, id);

                if (row == null)
                    return Content("<div class='alert alert-warning text-center'>⚠️ Cliente no encontrado.</div>", "text/html");

                var cliente = MapearCliente(row);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_DetailsPartial", cliente);

                return View(cliente);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger text-center'>❌ Error al cargar detalles:<br>{ex.Message}</div>", "text/html");
            }
        }

        // =====================================================
        // ELIMINAR CLIENTE - GET (muestra modal con datos)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var dt = _db.EjecutarSPDataTable("sp_ListarClientes", null);
                var row = BuscarClientePorId(dt, id);

                if (row == null)
                    return Content("<div class='alert alert-warning text-center'>⚠️ Cliente no encontrado.</div>", "text/html");

                var cliente = MapearCliente(row);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_DeletePartial", cliente);

                return View(cliente);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger text-center'>❌ Error al cargar los datos:<br>{ex.Message}</div>", "text/html");
            }
        }

        // =====================================================
        // ELIMINAR CLIENTE - POST CONFIRMADO (usa SP)
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdCliente)
        {
            var parametros = new Dictionary<string, object> { { "@IdCliente", IdCliente } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarCliente", parametros);
                return Json(new { success = true, message = "Cliente eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // =====================================================
        // 🔹 MÉTODOS PRIVADOS AUXILIARES
        // =====================================================
        private static DataRow BuscarClientePorId(DataTable dt, int id)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (Convert.ToInt32(r["IdCliente"]) == id)
                    return r;
            }
            return null;
        }

        private static Cliente MapearCliente(DataRow row)
        {
            return new Cliente
            {
                IdCliente = Convert.ToInt32(row["IdCliente"]),
                Nombre = row["Nombre"].ToString(),
                Apellidos = row["Apellidos"].ToString(),
                Contacto = row["Contacto"] == DBNull.Value ? "" : row["Contacto"].ToString(),
                FechaRegistro = DateOnly.FromDateTime(Convert.ToDateTime(row["FechaRegistro"])),
                Estado = Convert.ToBoolean(row["Estado"])
            };
        }
    }
}
