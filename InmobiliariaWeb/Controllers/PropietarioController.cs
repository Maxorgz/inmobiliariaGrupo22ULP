using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly DataContext _context;

        public PropietarioController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lista = _context.Propietarios.ToList();
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
                _context.Propietarios.Add(p);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }
    }
}