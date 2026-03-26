using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using BlogCoreSolution.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SlidersController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Create(Slider slider)
        {
            if (ModelState.IsValid)
            { // Los campos obligatorios se cumplieron.
                string rutaPrincipal = _webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (archivos.Count > 0)
                { // Crear un nuevo slider.
                    // Nos aseguramos de que tenga un nombre único siempre todas las veces vía un GUID (Globally Unique Identifier).
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"img\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName); // Obtiene la extensión del primer archivo.

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams); // Copia el archivo subido a la ruta calculada.
                    }

                    slider.UrlImagen = @"\img\sliders\" + nombreArchivo + extension;
                    _contenedorTrabajo.Slider.Add(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                { // Si hubo algún error, entonces vamos a agregarlo al modelo.
                    ModelState.AddModelError("Imagen", "Debes seleccionar una imagen");
                }
            }
            
            return View(slider);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            { // Se recibió el Id satisfactoriamente.
                Slider slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
                return View(slider);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (ModelState.IsValid)
            { // Los campos obligatorios se cumplieron.
                string rutaPrincipal = _webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                var sliderDesdeBd = _contenedorTrabajo.Slider.Get(slider.Id);

                if (archivos.Count > 0) // Hay al menos 1 archivo subido en la solicitud.
                { // Nueva imagen para el slider.
                    // Nos aseguramos de que tenga un nombre único siempre todas las veces vía un GUID (Globally Unique Identifier).
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"img\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName); // Obtiene la extensión del primer archivo.

                    var rutaImagen = Path.Combine(rutaPrincipal, sliderDesdeBd.UrlImagen.TrimStart('\\'));

                    // Validamos que el archivo (la imagen) ya exista.
                    if (System.IO.File.Exists(rutaImagen))
                    {   // Si ya existe, lo tenemos que borrar. Borrar la antigua para quedarnos únicamente con la nueva.
                        System.IO.File.Delete(rutaImagen);
                    }

                    // Subimos al sistema de archivos la nueva imagen.
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams); // Copia el archivo subido a la ruta calculada.
                    }

                    slider.UrlImagen = @"\img\sliders\" + nombreArchivo + extension;
                }
                else
                { // La imagen ya existía y se conserva esa versión.
                    slider.UrlImagen = sliderDesdeBd.UrlImagen;
                }

                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(slider);
        }

        #region Llamadas a la API
        // Llamadas a la API (Unit of Work).
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Slider.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Slider.Get(id);
            string rutaPrincipal = _webHostEnvironment.WebRootPath; // La usamos para encontrar la imagen que queremos eliminar del servidor.
            var rutaImagen = Path.Combine(rutaPrincipal, objFromDb.UrlImagen.TrimStart('\\'));

            // Verifico si ya existe el archivo de imagen.
            if (System.IO.File.Exists(rutaImagen))
            { // Borramos la imagen del servidor.
                System.IO.File.Delete(rutaImagen);
            }

            if (objFromDb == null)
            {   // En caso de que no se encuentre el artículo en la BBDD.
                return Json(new { success = false, message = "Error borrando slider" });
            }

            _contenedorTrabajo.Slider.Remove(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Slider borrado correctamente" });
        }
        #endregion
    }
}
