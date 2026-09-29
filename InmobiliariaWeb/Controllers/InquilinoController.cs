using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models; 
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Repositorio; 


namespace InmobiliariaWeb.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino _repoInquilino;

        public InquilinoController(IConfiguration configuration)
        {
            _repoInquilino = new RepositorioInquilino(configuration);
        }


        public IActionResult Index(int pagina = 1, int tamanio = 10)
        {
            if (tamanio != 5 && tamanio != 10 && tamanio != 20) tamanio = 10;
            int totalRegistros = _repoInquilino.ObtenerTotal();
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (decimal)tamanio));
            pagina = Math.Clamp(pagina, 1, totalPaginas);
            var propietarios = _repoInquilino.ObtenerLista(pagina, tamanio);

            ViewBag.PaginaActual = pagina;
            ViewBag.TamanioPagina = tamanio;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(propietarios);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repoInquilino.Alta(inquilino); 
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repoInquilino.Modificacion(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }
        
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmado(int IdInquilino)
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