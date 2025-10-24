using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    [Authorize]
    public class VehiculosController : Controller
    {
        private readonly DatabaseService _db;
        public VehiculosController(DatabaseService db)
        {
            _db = db;
        }

        // ======================
        // LISTAR
        // ======================
        public IActionResult Index()
        {
            var vehiculos = _db.EjecutarSPDataTable("sp_ListarVehiculos", null);
            ViewBag.Vehiculos = vehiculos;
            return View();
        }

        // ======================
        // CREAR (GET)
        // ======================
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var vehiculo = new Vehiculo { Estado = true };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_CreatePartial", vehiculo);

            return View(vehiculo);
        }

        // ======================
        // CREAR (POST)
        // ======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create([FromForm] Vehiculo model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreatePartial", model);

            var parametros = new Dictionary<string, object>
            {
                { "@Marca", model.Marca.Trim() },
                { "@Modelo", model.Modelo.Trim() },
                { "@Transmision", model.Transmision?.Trim() },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarVehiculo", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ======================
        // EDITAR (GET)
        // ======================
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarVehiculos", null);
            var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["IdVehiculo"]) == id);

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Vehículo no encontrado.</div>", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                Marca = row["Marca"].ToString(),
                Modelo = row["Modelo"].ToString(),
                Transmision = row["Transmision"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditPartial", vehiculo);

            return View(vehiculo);
        }

        // ======================
        // EDITAR (POST)
        // ======================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit([FromForm] Vehiculo model)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@IdVehiculo", model.IdVehiculo },
                { "@Marca", model.Marca.Trim() },
                { "@Modelo", model.Modelo.Trim() },
                { "@Transmision", model.Transmision?.Trim() },
                { "@Estado", model.Estado }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarVehiculo", parametros);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ======================
        // DETALLES
        // ======================
        public IActionResult Details(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarVehiculos", null);
            var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["IdVehiculo"]) == id);

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Vehículo no encontrado.</div>", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                Marca = row["Marca"].ToString(),
                Modelo = row["Modelo"].ToString(),
                Transmision = row["Transmision"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DetailsPartial", vehiculo);

            return View(vehiculo);
        }

        // ======================
        // ELIMINAR
        // ======================
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarVehiculos", null);
            var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["IdVehiculo"]) == id);

            if (row == null)
                return Content("<div class='alert alert-warning text-center'>⚠️ Vehículo no encontrado.</div>", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = Convert.ToInt32(row["IdVehiculo"]),
                Marca = row["Marca"].ToString(),
                Modelo = row["Modelo"].ToString(),
                Transmision = row["Transmision"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DeletePartial", vehiculo);

            return View(vehiculo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdVehiculo)
        {
            var parametros = new Dictionary<string, object> { { "@IdVehiculo", IdVehiculo } };

            try
            {
                _db.EjecutarSPNonQuery("sp_EliminarVehiculo", parametros);
                return Json(new { success = true, message = "Vehículo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
