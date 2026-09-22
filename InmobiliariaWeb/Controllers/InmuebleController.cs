using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioTipoInmueble repoTipo;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly ILogger<InmuebleController> logger;

        public InmuebleController(
            IRepositorioInmueble repo,
            IRepositorioTipoInmueble repoTipo,
            IRepositorioPropietario repoPropietario,
            ILogger<InmuebleController> logger)
        {
            this.repositorio = repo;
            this.repoTipo = repoTipo;
            this.repoPropietario = repoPropietario;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (!ModelState.IsValid) return View(entidad);
                repositorio.Alta(entidad);
                TempData["Mensaje"] = "Inmueble creado correctamente";
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
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null) return NotFound();
                if (!ModelState.IsValid) return View(entidad);

                i.Direccion = entidad.Direccion;
                i.Cupo = entidad.Cupo;
                i.IdTipoInmueble = entidad.IdTipoInmueble;
                i.Latitud = entidad.Latitud;
                i.Longitud = entidad.Longitud;
                i.PrecioPorDia = entidad.PrecioPorDia;
                i.PorcentajeReserva = entidad.PorcentajeReserva;
                i.IdPropietario = entidad.IdPropietario;
                repositorio.Modificacion(i);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Suspender(int id)
        {
            repositorio.CambiarDisponibilidad(id, false);
            TempData["Mensaje"] = "Inmueble suspendido de la oferta";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reactivar(int id)
        {
            repositorio.CambiarDisponibilidad(id, true);
            TempData["Mensaje"] = "Inmueble reactivado en la oferta";
            return RedirectToAction(nameof(Index));
        }
    }
}