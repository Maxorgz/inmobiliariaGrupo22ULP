using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
using InmobiliariaWeb.Models; 

namespace InmobiliariaWeb.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino _repoInquilino;

        public InquilinoController(IConfiguration configuration)
        {
            _repoInquilino = new RepositorioInquilino(configuration);
        }

        public IActionResult Index()
        {
            var lista = _repoInquilino.ObtenerTodos(); 
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repoInquilino.Alta(inquilino); 
                return RedirectToAction("Index");
            }
            return View(inquilino);
        }
    }
}