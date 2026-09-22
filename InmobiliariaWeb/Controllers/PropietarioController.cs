using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaWeb.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario _repoPropietario;

        public PropietarioController(IConfiguration configuration)
        {
            _repoPropietario = new RepositorioPropietario(configuration);
        }


        [Authorize]
        public IActionResult Index(int pagina = 1, int tamanio = 10)
        {
            if (tamanio != 5 && tamanio != 10 && tamanio != 20) tamanio = 10;
            int totalRegistros = _repoPropietario.ObtenerTotal();
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (decimal)tamanio));
            pagina = Math.Clamp(pagina, 1, totalPaginas);
            var propietarios = _repoPropietario.ObtenerTodos(pagina, tamanio);

            ViewBag.PaginaActual = pagina;
            ViewBag.TamanioPagina = tamanio;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(propietarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Propietario p)
        {
            if (ModelState.IsValid)
            {
                _repoPropietario.Alta(p);
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public IActionResult Edit(int id)
        {
            var propietario = _repoPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Propietario p)
        {
            ModelState.Remove("Clave");

            if (ModelState.IsValid)
            {
                _repoPropietario.Modificacion(p);
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public IActionResult Delete(int id)
        {
            var propietario = _repoPropietario.ObtenerPorId(id);
            if (propietario == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmado(int IdPropietario)
        {
            try
            {
                _repoPropietario.Baja(IdPropietario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}