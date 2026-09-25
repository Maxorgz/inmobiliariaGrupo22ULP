using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioReserva repoReserva;
        private readonly ILogger<PagoController> logger;
        private const int IdUsuarioTemporal = 1;

        public PagoController(
            IRepositorioPago repo,
            IRepositorioReserva repoReserva,
            ILogger<PagoController> logger)
        {
            this.repositorio = repo;
            this.repoReserva = repoReserva;
            this.logger = logger;
        }

        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamano = 5;
                var lista = repositorio.ObtenerTodos(Math.Max(pagina, 1), tamano);
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

        public ActionResult Create(int idReserva)
        {
            var reserva = repoReserva.ObtenerPorId(idReserva);
            if (reserva == null) return NotFound();

            var pago = new Pago { IdReserva = idReserva, FechaPago = DateTime.Today };
            ViewBag.Reserva = reserva;
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago entidad)
        {
            try
            {
                if (!ModelState.IsValid) return View(entidad);

                entidad.IdUsuarioCreador = IdUsuarioTemporal;
                repositorio.Alta(entidad);
                TempData["Mensaje"] = "Pago registrado correctamente";
                return RedirectToAction("Details", "Reserva", new { id = entidad.IdReserva });
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
        public ActionResult Edit(int id, Pago entidad)
        {
            try
            {
                var p = repositorio.ObtenerPorId(id);
                if (p == null) return NotFound();

                if (string.IsNullOrWhiteSpace(entidad.Concepto))
                {
                    ModelState.AddModelError("", "El concepto es obligatorio");
                    return View(p);
                }

                p.Concepto = entidad.Concepto;
                repositorio.Modificacion(p);
                TempData["Mensaje"] = "Concepto actualizado correctamente";
                return RedirectToAction("Details", "Reserva", new { id = p.IdReserva });
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
        public ActionResult Anular(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();

            repositorio.Anular(id, IdUsuarioTemporal);
            TempData["Mensaje"] = "Pago anulado correctamente";
            return RedirectToAction("Details", "Reserva", new { id = p.IdReserva });
        }
    }
}