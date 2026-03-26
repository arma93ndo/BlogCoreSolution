using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment webHostEnvironment)
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
            ArticuloVM viewModel = new ArticuloVM
            {
                Articulo = new BlogCoreSolution.Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM viewModel)
        {
            if (ModelState.IsValid)
            { // Los campos obligatorios se cumplieron.
                string rutaPrincipal = _webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (viewModel.Articulo.Id == 0 && archivos.Count > 0) // Se creó un Artículo con los valores por defecto y hay al menos 1 archivo subido en la solicitud.
                { // Crear un nuevo artículo.
                    // Nos aseguramos de que tenga un nombre único siempre todas las veces vía un GUID (Globally Unique Identifier).
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"img\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName); // Obtiene la extensión del primer archivo.

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams); // Copia el archivo subido a la ruta calculada.
                    }

                    viewModel.Articulo.UrlImagen = @"\img\articulos\" + nombreArchivo + extension;
                    _contenedorTrabajo.Articulo.Add(viewModel.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                { // Si hubo algún error, entonces vamos a agregarlo al modelo.
                    ModelState.AddModelError("Imagen", "Debes seleccionar una imagen");
                }
            }
            // Recargamos las opciones de la base de datos para enviarlas (de nuevo) al formulario.
            viewModel.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloVM viewModel = new ArticuloVM
            {
                Articulo = new BlogCoreSolution.Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria()
            };

            if (id != null)
            { // Se recibió el Id satisfactoriamente.
                viewModel.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM viewModel)
        {
            if (ModelState.IsValid)
            { // Los campos obligatorios se cumplieron.
                string rutaPrincipal = _webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(viewModel.Articulo.Id);

                if (archivos.Count > 0) // Hay al menos 1 archivo subido en la solicitud.
                { // Nueva imagen para el artículo.
                    // Nos aseguramos de que tenga un nombre único siempre todas las veces vía un GUID (Globally Unique Identifier).
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"img\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName); // Obtiene la extensión del primer archivo.
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeBd.UrlImagen.TrimStart('\\'));

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

                    viewModel.Articulo.UrlImagen = @"\img\articulos\" + nombreArchivo + extension;
                }
                else
                { // La imagen ya existía y se conserva esa versión.
                    viewModel.Articulo.UrlImagen = articuloDesdeBd.UrlImagen;
                }

                _contenedorTrabajo.Articulo.Update(viewModel.Articulo);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            // Recargamos las opciones de la base de datos para enviarlas (de nuevo) al formulario.
            viewModel.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria();
            return View(viewModel);
        }


        #region Llamadas a la API
        // Llamadas a la API (Unit of Work).
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includePropierties: "Categoria") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Articulo.Get(id);
            string rutaPrincipal = _webHostEnvironment.WebRootPath; // La usamos para encontrar la imagen que queremos eliminar del servidor.
            var rutaImagen = Path.Combine(rutaPrincipal, objFromDb.UrlImagen.TrimStart('\\'));

            // Verifico si ya existe el archivo de imagen.
            if (System.IO.File.Exists(rutaImagen))
            { // Borramos la imagen del servidor.
                System.IO.File.Delete(rutaImagen);
            }
            
            if (objFromDb == null)
            {   // En caso de que no se encuentre el artículo en la BBDD.
                return Json(new { success = false, message = "Error borrando artículo" });
            }

            _contenedorTrabajo.Articulo.Remove(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Artículo borrado correctamente" });
        }
        #endregion
    }
}
