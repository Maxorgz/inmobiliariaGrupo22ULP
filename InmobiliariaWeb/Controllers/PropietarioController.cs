using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario _repoPropietario;

        public PropietarioController(IConfiguration configuration)
        {
            _repoPropietario = new RepositorioPropietario(configuration);
        }

        public IActionResult Index()
        {
            var lista = _repoPropietario.ObtenerTodos();
            return View(lista);
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