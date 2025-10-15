// using Microsoft.AspNetCore.Mvc;
// using LiberiaDriveMVC.Services;
// using System.Data;

// namespace LiberiaDriveMVC.Controllers
// {
//     public class InstructoresController : Controller
//     {
//         private readonly DatabaseService _db;

//         public InstructoresController(DatabaseService db)
//         {
//             _db = db;
//         }

//         // GET: Instructores
//         public IActionResult Index()
//         {
//             DataTable instructores = _db.EjecutarSP("sp_ListarInstructores");
//             return View(instructores);
//         }

//         // GET: Crear
//         public IActionResult Create()
//         {
//             return View();
//         }

//         [HttpPost]
//         public IActionResult Create(string nombre, string apellidos, bool estado = true)
//         {
//             var parametros = new Dictionary<string, object>
//             {
//                 { "@Nombre", nombre },
//                 { "@Apellidos", apellidos },
//                 { "@Estado", estado }
//             };

//             _db.EjecutarSPNonQuery("sp_InsertarInstructor", parametros);
//             return RedirectToAction("Index");
//         }

//         // GET: Editar
//         public IActionResult Edit(int id)
//         {
//             var parametros = new Dictionary<string, object> { { "@IdInstructor", id } };
//             DataTable dt = _db.EjecutarSP("sp_ObtenerInstructorPorId", parametros);
//             if (dt.Rows.Count == 0) return NotFound();
//             ViewBag.Instructor = dt.Rows[0];
//             return View();
//         }

//         [HttpPost]
//         public IActionResult Edit(int id, string nombre, string apellidos, bool estado)
//         {
//             var parametros = new Dictionary<string, object>
//             {
//                 { "@IdInstructor", id },
//                 { "@Nombre", nombre },
//                 { "@Apellidos", apellidos },
//                 { "@Estado", estado }
//             };
//             _db.EjecutarSPNonQuery("sp_ActualizarInstructor", parametros);
//             return RedirectToAction("Index");
//         }

//         public IActionResult Delete(int id)
//         {
//             var parametros = new Dictionary<string, object> { { "@IdInstructor", id } };
//             _db.EjecutarSPNonQuery("sp_EliminarInstructor", parametros);
//             return RedirectToAction("Index");
//         }
//     }
// }
