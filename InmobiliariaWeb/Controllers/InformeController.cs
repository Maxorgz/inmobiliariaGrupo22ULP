using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InformeController : Controller
    {
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioReserva repoReserva;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioPago repoPago;

        public InformeController(
            IRepositorioInmueble repoInmueble,
            IRepositorioReserva repoReserva,
            IRepositorioPropietario repoPropietario,
            IRepositorioPago repoPago)
        {
            this.repoInmueble = repoInmueble;
            this.repoReserva = repoReserva;
            this.repoPropietario = repoPropietario;
            this.repoPago = repoPago;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inmuebles(bool? disponible)
        {
            ViewBag.Disponible = disponible;
            var lista = repoInmueble.ObtenerTodosConDetalle(disponible);
            return View(lista);
        }

        public ActionResult InmueblesPorPropietario(int? idPropietario)
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            ViewBag.IdPropietario = idPropietario;
            var lista = idPropietario.HasValue
                ? repoInmueble.ObtenerPorPropietario(idPropietario.Value)
                : new List<Inmueble>();
            return View(lista);
        }

        public ActionResult MasReservados()
        {
            var lista = repoInmueble.MasReservados(365);
            return View(lista);
        }

        public ActionResult SinReservas(int dias = 30)
        {
            ViewBag.Dias = dias;
            var lista = repoInmueble.SinReservas(dias);
            return View(lista);
        }

        public ActionResult ReservasVigentes()
        {
            var lista = repoReserva.ObtenerVigentes();
            return View(lista);
        }

        public ActionResult ReservasPorVencer(int dias = 7)
        {
            ViewBag.Dias = dias;
            var lista = repoReserva.ObtenerPorVencer(dias);
            return View(lista);
        }

        public ActionResult InmueblesDisponibles(DateTime? desde, DateTime? hasta)
        {
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            var lista = (desde.HasValue && hasta.HasValue)
                ? repoInmueble.ObtenerDisponiblesEntreFechas(desde.Value, hasta.Value)
                : new List<Inmueble>();
            return View(lista);
        }

        public ActionResult PagosPorReserva(int? idReserva)
        {
            ViewBag.IdReserva = idReserva;
            var lista = idReserva.HasValue
                ? repoPago.ObtenerPorReserva(idReserva.Value)
                : new List<Pago>();
            return View(lista);
        }
    }
}