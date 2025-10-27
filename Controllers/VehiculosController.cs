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
       public IActionResult Index(string searchPlaca = "")
{
    var parametros = new Dictionary<string, object>();

    if (!string.IsNullOrWhiteSpace(searchPlaca))
        parametros.Add("@Placa", searchPlaca.Trim());

    var vehiculos = _db.EjecutarSPDataTable(
        string.IsNullOrWhiteSpace(searchPlaca)
        ? "sp_ListarVehiculos"
        : "sp_BuscarVehiculosPorPlaca",
        parametros.Count > 0 ? parametros : null
    );

    ViewBag.SearchPlaca = searchPlaca;
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
            return PartialView("_CreatePartial", vehiculo);
        }

        // ======================
        // CREAR (POST)
        // ======================
        [HttpPost]
[Authorize(Roles = "Administrador")]
public IActionResult Create([FromForm] Vehiculo model)
        {
    // Verificar si existe una placa igual
var placaParams = new Dictionary<string, object> { { "@Placa", model.Placa.Trim() } };
var existePlaca = _db.EjecutarSPDataTable("SELECT 1 FROM Vehiculo WHERE Placa = @Placa", placaParams, true);

if (existePlaca.Rows.Count > 0)
{
    return Json(new
    {
        success = false,
        message = "⚠️ Ya existe un vehículo registrado con esa placa."
    });
}

    try
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Datos inválidos en el formulario." });

        var parametros = new Dictionary<string, object>
        {
            { "@Marca", model.Marca.Trim() },
            { "@Modelo", model.Modelo.Trim() },
            { "@Transmision", model.Transmision.Trim() },
            { "@Estado", model.Estado },
            { "@Placa", model.Placa.Trim() }
        };

        _db.EjecutarSPNonQuery("sp_InsertarVehiculo", parametros);

        return Json(new { success = true, message = "Vehículo guardado correctamente." });
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
                return Content("❌ Vehículo no encontrado.", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = (int)row["IdVehiculo"],
                Marca = row["Marca"].ToString()!,
                Modelo = row["Modelo"].ToString()!,
                Transmision = row["Transmision"].ToString()!,
                Estado = Convert.ToBoolean(row["Estado"]),
                Placa = row["Placa"].ToString()!
            };

            return PartialView("_EditPartial", vehiculo);
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
                { "@Marca", model.Marca },
                { "@Modelo", model.Modelo },
                { "@Transmision", model.Transmision },
                { "@Estado", model.Estado },
                { "@Placa", model.Placa }
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
                return Content("❌ Vehículo no encontrado.", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = (int)row["IdVehiculo"],
                Marca = row["Marca"].ToString()!,
                Modelo = row["Modelo"].ToString()!,
                Transmision = row["Transmision"].ToString()!,
                Estado = Convert.ToBoolean(row["Estado"]),
                Placa = row["Placa"].ToString()!
            };

            return PartialView("_DetailsPartial", vehiculo);
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
                return Content("❌ Vehículo no encontrado.", "text/html");

            var vehiculo = new Vehiculo
            {
                IdVehiculo = (int)row["IdVehiculo"],
                Marca = row["Marca"].ToString()!,
                Modelo = row["Modelo"].ToString()!,
                Transmision = row["Transmision"].ToString()!,
                Estado = Convert.ToBoolean(row["Estado"]),
                Placa = row["Placa"].ToString()!
            };

            return PartialView("_DeletePartial", vehiculo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int IdVehiculo)
        {
            var parametros = new Dictionary<string, object> { { "@IdVehiculo", IdVehiculo } };

            _db.EjecutarSPNonQuery("sp_EliminarVehiculo", parametros);
            return Json(new { success = true });
        }
        
    }
}
