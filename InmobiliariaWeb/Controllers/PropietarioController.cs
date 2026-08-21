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
                return RedirectToAction("Index");
            }
            return View(p);
        }
    }
}