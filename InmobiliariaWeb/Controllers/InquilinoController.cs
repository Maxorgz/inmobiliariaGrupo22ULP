using Microsoft.AspNetCore.Mvc;
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repoInquilino.Alta(inquilino); 
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        public IActionResult Edit(int id)
        {
            var inquilino = _repoInquilino.ObtenerPorId(id);
            if (inquilino == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repoInquilino.Modificacion(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        public IActionResult Delete(int id)
        {
            var inquilino = _repoInquilino.ObtenerPorId(id);
            if (inquilino == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmado(int IdInquilino) // ¡Importante que se llame igual que tu ID!
        {
            try
            {
                _repoInquilino.Baja(IdInquilino);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}