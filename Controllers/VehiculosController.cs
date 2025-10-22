using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Services;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly DatabaseService _db;

        public VehiculosController(DatabaseService db)
        {
            _db = db;
        }

        // ✅ LISTAR VEHÍCULOS
        public IActionResult Index()
        {
            DataTable vehiculos = _db.EjecutarSPDataTable("sp_ListarVehiculos");  // 🔄 corregido
            return View(vehiculos);
        }

        // ✅ MOSTRAR FORMULARIO DE CREACIÓN
        public IActionResult Create() => View();

        // ✅ GUARDAR NUEVO VEHÍCULO
        [HttpPost]
        public IActionResult Create(string marca, string modelo, string transmision, bool estado = true)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@Marca", marca },
                { "@Modelo", modelo },
                { "@Transmision", transmision },
                { "@Estado", estado }
            };

            _db.EjecutarSPNonQuery("sp_InsertarVehiculo", parametros);  // 🔄 corregido
            TempData["Success"] = "✅ Vehículo agregado correctamente.";
            return RedirectToAction("Index");
        }

        // ✅ MOSTRAR FORMULARIO DE EDICIÓN
        public IActionResult Edit(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
            DataTable dt = _db.EjecutarSPDataTable("sp_ObtenerVehiculoPorId", parametros);  // 🔄 corregido
            if (dt.Rows.Count == 0) return NotFound();
            ViewBag.Vehiculo = dt.Rows[0];
            return View();
        }

        // ✅ GUARDAR CAMBIOS DE EDICIÓN
        [HttpPost]
        public IActionResult Edit(int id, string marca, string modelo, string transmision, bool estado)
        {
            var parametros = new Dictionary<string, object>
            {
                { "@IdVehiculo", id },
                { "@Marca", marca },
                { "@Modelo", modelo },
                { "@Transmision", transmision },
                { "@Estado", estado }
            };

            _db.EjecutarSPNonQuery("sp_ActualizarVehiculo", parametros);  // 🔄 corregido
            TempData["Success"] = "✏️ Vehículo actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // ✅ ELIMINAR VEHÍCULO
        public IActionResult Delete(int id)
        {
            var parametros = new Dictionary<string, object> { { "@IdVehiculo", id } };
            _db.EjecutarSPNonQuery("sp_EliminarVehiculo", parametros);  // 🔄 corregido
            TempData["Success"] = "🗑️ Vehículo eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
