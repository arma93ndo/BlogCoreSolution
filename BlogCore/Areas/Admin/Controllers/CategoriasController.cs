using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            { 
                // Lógica para guardar en la BBDD.
                _contenedorTrabajo.Categoria.Add(categoria);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            // Si hubieron problemas con el modelo, deja al usuario en el mismo formulario.
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Categoria categoria = new Categoria();
            categoria = _contenedorTrabajo.Categoria.Get(id);

            // Verificamos que sí se haya hallado la categoría.
            if (categoria == null)
            { 
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Lógica para guardar en la BBDD.
                _contenedorTrabajo.Categoria.Update(categoria);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            // Si hubieron problemas con el modelo, deja al usuario en el mismo formulario.
            return View(categoria);
        }

        #region Llamadas a la API
        // Llamadas a la API (Unit of Work).
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Categoria.GetAll() } );
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Categoria.Get(id);

            if (objFromDb == null)
            {   // En caso de que no se encuentre la categoría en la BBDD.
                return Json(new { success = false, message = "Error borrando categoría" });
            }

            _contenedorTrabajo.Categoria.Remove(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Categoría borrada correctamente" });
        }
        #endregion
    }
}
