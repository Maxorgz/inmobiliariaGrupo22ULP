using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly ILogger<TipoInmuebleController> logger;

        public TipoInmuebleController(IRepositorioTipoInmueble repo, ILogger<TipoInmuebleController> logger)
        {
            this.repositorio = repo;
            this.logger = logger;
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
            return View(entidad);
        }

        [Route("[controller]/Buscar/{q}", Name = "BuscarTipoInmueble")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorDescripcion(q);
                return Json(new { Datos = res });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoInmueble entidad)
        {
            try
            {
                if (!ModelState.IsValid) return View(entidad);
                repositorio.Alta(entidad);
                TempData["Mensaje"] = "Tipo de inmueble creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                return View(entidad);
            }
        }

        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TipoInmueble entidad)
        {
            try
            {
                var t = repositorio.ObtenerPorId(id);
                if (t == null) return NotFound();
                if (!ModelState.IsValid) return View(entidad);

                t.Descripcion = entidad.Descripcion;
                repositorio.Modificacion(t);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                return View(entidad);
            }
        }
    }
}