using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioTipoInmueble repoTipo;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioImagen repoImagen;
        private readonly ILogger<InmuebleController> logger;

        private readonly IWebHostEnvironment environment;

        public InmuebleController(
            IRepositorioInmueble repo,
            IRepositorioTipoInmueble repoTipo,
            IRepositorioPropietario repoPropietario,
            IRepositorioImagen repoImagen,
            ILogger<InmuebleController> logger,
            IWebHostEnvironment environment
            )
        {
            this.repositorio = repo;
            this.repoTipo = repoTipo;
            this.repoPropietario = repoPropietario;
            this.repoImagen = repoImagen;
            this.logger = logger;
            this.environment = environment;
        }

        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamano = 5;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamano);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamano == 0 ? total / tamano : total / tamano + 1;

                foreach (var inmueble in lista)
                {
                    var imagenes = repoImagen.BuscarPorInmueble(inmueble.IdInmueble);
                    if (imagenes.Any())
                    {
                        inmueble.RutaImagen = imagenes.First().Url;
                    }
                }

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index");
                throw;
            }
        }

        public ActionResult Details(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            ViewBag.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        [Route("[controller]/BuscarTipo/{q}", Name = "BuscarTipoParaInmueble")]
        public IActionResult BuscarTipo(string q)
        {
            var res = repoTipo.BuscarPorDescripcion(q);
            return Json(new { Datos = res });
        }

        [Route("[controller]/BuscarPropietario/{q}", Name = "BuscarPropietarioParaInmueble")]
        public IActionResult BuscarPropietario(string q)
        {
            var res = repoPropietario.BuscarPorNombre(q);
            return Json(new { Datos = res });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ObtenerCatalogoJson(string? busqueda = null, decimal? precioMaximo = null)
        {
            try
            {
                var inmuebles = repositorio.ObtenerTodos();
                var query = inmuebles.AsQueryable();
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    var filtro = busqueda.ToLower();
                    query = query.Where(i =>
                        (i.Direccion != null && i.Direccion.ToLower().Contains(filtro)) ||
                        (i.TipoInmuebleDescripcion != null && i.TipoInmuebleDescripcion.ToLower().Contains(filtro))
                    );
                }

                if (precioMaximo.HasValue && precioMaximo.Value > 0)
                {
                    query = query.Where(i => i.PrecioPorDia <= precioMaximo.Value);
                }

                var inmueblesFiltrados = query.ToList();

                var resultado = inmueblesFiltrados.Select(i =>
                {
                    var imagenes = repoImagen.BuscarPorInmueble(i.IdInmueble) as IEnumerable<Imagen>; ;
                    var rutaPrincipal = imagenes?.FirstOrDefault()?.Url ?? "";

                    return new
                    {
                        i.IdInmueble,
                        i.Direccion,
                        i.PrecioPorDia,
                        TipoInmuebleDescripcion = i.TipoInmuebleDescripcion ?? "Inmueble",
                        RutaImagen = rutaPrincipal
                    };
                });

                return Json(new { Datos = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create(Inmueble entidad)
        {
            try
            {
                ModelState.Remove("Propietario");
                ModelState.Remove("TipoInmueble");

                if (!ModelState.IsValid)
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
                    return View(entidad);
                }

                repositorio.Alta(entidad);

                if (entidad.ArchivoImagen != null && entidad.ArchivoImagen.Length > 0)
                {
                    string uploadsFolder = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", entidad.IdInmueble.ToString());
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var extension = Path.GetExtension(entidad.ArchivoImagen.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(uploadsFolder, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await entidad.ArchivoImagen.CopyToAsync(stream);
                    }

                    Imagen img = new Imagen
                    {
                        IdInmueble = entidad.IdInmueble,
                        Url = $"/Uploads/Inmuebles/{entidad.IdInmueble}/{nombreArchivo}"
                    };
                    repoImagen.Alta(img);
                }

                TempData["Mensaje"] = "Inmueble creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                return View(entidad);
            }
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
            ViewBag.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int id, Inmueble entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null) return NotFound();

                ModelState.Remove("Propietario");
                ModelState.Remove("TipoInmueble");

                if (!ModelState.IsValid)
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
                    ViewBag.Imagenes = repoImagen.BuscarPorInmueble(id);
                    return View(entidad);
                }

                i.Direccion = entidad.Direccion;
                i.Cupo = entidad.Cupo;
                i.IdTipoInmueble = entidad.IdTipoInmueble;
                i.Latitud = entidad.Latitud;
                i.Longitud = entidad.Longitud;
                i.PrecioPorDia = entidad.PrecioPorDia;
                i.PorcentajeReserva = entidad.PorcentajeReserva;
                i.IdPropietario = entidad.IdPropietario;

                repositorio.Modificacion(i);

                if (entidad.ArchivoImagen != null && entidad.ArchivoImagen.Length > 0)
                {
                    string uploadsFolder = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", id.ToString());
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var extension = Path.GetExtension(entidad.ArchivoImagen.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(uploadsFolder, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await entidad.ArchivoImagen.CopyToAsync(stream);
                    }

                    Imagen img = new Imagen
                    {
                        IdInmueble = id,
                        Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}"
                    };
                    repoImagen.Alta(img);
                }

                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.TiposInmueble = repoTipo.ObtenerTodos();
                ViewBag.Imagenes = repoImagen.BuscarPorInmueble(id);
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                return View(entidad);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Suspender(int id)
        {
            repositorio.CambiarDisponibilidad(id, false);
            TempData["Mensaje"] = "Inmueble suspendido de la oferta";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Reactivar(int id)
        {
            repositorio.CambiarDisponibilidad(id, true);
            TempData["Mensaje"] = "Inmueble reactivado en la oferta";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult BuscarInmueblesAjax(string q)
        {
            try
            {
                IList<Inmueble> inmuebles = new List<Inmueble>();
                if (!string.IsNullOrWhiteSpace(q))
                {
                    inmuebles = repositorio.BuscarPorDireccion(q);
                }

                var resultados = inmuebles.Select(i => new
                {
                    id = i.IdInmueble,
                    text = $"{i.Direccion} (Tipo: {i.TipoInmuebleDescripcion})"
                });

                return Json(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}