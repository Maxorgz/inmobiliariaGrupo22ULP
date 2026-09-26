using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Repositorio;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repoInquilino;
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioPago repoPago;
        private readonly ILogger<ReservaController> logger;
        private const int IdUsuarioTemporal = 1;
        public ReservaController(
            IRepositorioReserva repo,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble,
            IRepositorioPago repoPago,
            ILogger<ReservaController> logger)
        {
            this.repositorio = repo;
            this.repoInquilino = repoInquilino;
            this.repoInmueble = repoInmueble;
            this.repoPago = repoPago;
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
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
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
            ViewBag.Pagos = repoPago.ObtenerPorReserva(id);
            return View(entidad);
        }

        [Route("[controller]/BuscarInquilino/{q}", Name = "BuscarInquilinoParaReserva")]
        public IActionResult BuscarInquilino(string q)
        {
            var res = repoInquilino.BuscarPorNombre(q);
            return Json(new { Datos = res });
        }

        [HttpGet]
        public IActionResult BuscarInmuebleDisponible(DateTime desde, DateTime hasta)
        {
            var res = repoInmueble.ObtenerDisponiblesEntreFechas(desde, hasta);
            return Json(new { Datos = res });
        }

        public ActionResult Create()
        {
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva entidad)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                if (entidad.FechaHasta <= entidad.FechaDesde)
                {
                    ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio");
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                var solapa = repositorio.ExisteSolapamiento(
                    entidad.IdInmueble, entidad.FechaDesde, entidad.FechaHasta, idReservaExcluir: 0);
                if (solapa)
                {
                    ModelState.AddModelError("", "El inmueble ya se encuentra reservado en esas fechas");
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                var inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                if (inmueble == null)
                {
                    ModelState.AddModelError("", "El inmueble seleccionado no existe");
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }
                
                entidad.MontoPorDia = inmueble.PrecioPorDia;
                repositorio.Alta(entidad);

                int cantidadDias = (entidad.FechaHasta - entidad.FechaDesde).Days;
                decimal montoTotal = cantidadDias * inmueble.PrecioPorDia;
                decimal montoSenia = montoTotal * (inmueble.PorcentajeReserva / 100m);

                var senia = new Pago
                {
                    IdReserva = entidad.IdReserva,
                    Concepto = "Seña inicial",
                    FechaPago = DateTime.Today,
                    Importe = montoSenia,
                    IdUsuarioCreador = IdUsuarioTemporal
                };
                repoPago.Alta(senia);

                TempData["Mensaje"] = "Reserva creada correctamente, se generó la seña inicial";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                return View(entidad);
            }
        }

        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            
            return View(entidad);
        }

       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Reserva entidad)
        {
            try
            {
                var r = repositorio.ObtenerPorId(id);
                if (r == null) return NotFound();
                
                if (!ModelState.IsValid) 
                {
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                if (entidad.FechaHasta <= entidad.FechaDesde)
                {
                    ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio");
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                var solapa = repositorio.ExisteSolapamiento(
                    entidad.IdInmueble, entidad.FechaDesde, entidad.FechaHasta, idReservaExcluir: id);
                if (solapa)
                {
                    ModelState.AddModelError("", "El inmueble ya se encuentra reservado en esas fechas");
                    ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                    return View(entidad);
                }

                r.IdInquilino = entidad.IdInquilino;
                r.IdInmueble = entidad.IdInmueble;
                r.FechaDesde = entidad.FechaDesde;
                r.FechaHasta = entidad.FechaHasta;
                repositorio.Modificacion(r);
                
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                return View(entidad);
            }
        }

        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id); 
                TempData["Mensaje"] = "Reserva anulada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Delete");
                TempData["Error"] = "Ocurrió un error al intentar anular la reserva.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}