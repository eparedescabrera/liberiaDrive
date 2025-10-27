using Microsoft.AspNetCore.Mvc;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Services;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    public class LicenciasController : Controller
    {
        private readonly DatabaseService _db;

        public LicenciasController(DatabaseService db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var data = _db.EjecutarSPDataTable("sp_ListarLicencias", null);
            ViewBag.Licencias = data;
            return View();
        }

        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new Licencia());
        }

        [HttpPost]
        public IActionResult Create([FromForm] Licencia model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreatePartial", model);

            var param = new Dictionary<string, object>
            {
                { "@TipoLicencia", model.TipoLicencia.Trim() }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_InsertarLicencia", param);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarLicencias", null);
            var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["IdLicencia"]) == id);

            if (row == null)
                return Content("No encontrado");

            var licencia = new Licencia
            {
                IdLicencia = id,
                TipoLicencia = row["TipoLicencia"].ToString()
            };

            return PartialView("_EditPartial", licencia);
        }

        [HttpPost]
        public IActionResult Edit(Licencia model)
        {
            var param = new Dictionary<string, object>
            {
                { "@IdLicencia", model.IdLicencia },
                { "@TipoLicencia", model.TipoLicencia.Trim() }
            };

            try
            {
                _db.EjecutarSPNonQuery("sp_ActualizarLicencia", param);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Delete(int id)
        {
            var dt = _db.EjecutarSPDataTable("sp_ListarLicencias", null);
            var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["IdLicencia"]) == id);

            if (row == null)
                return Content("No encontrado");

            return PartialView("_DeletePartial",
                new Licencia
                {
                    IdLicencia = id,
                    TipoLicencia = row["TipoLicencia"].ToString()
                });
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var param = new Dictionary<string, object>
                {
                    { "@IdLicencia", id }
                };

                _db.EjecutarSPNonQuery("sp_EliminarLicencia", param);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
