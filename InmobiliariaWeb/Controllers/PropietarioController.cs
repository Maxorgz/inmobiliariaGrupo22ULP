using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaWeb.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario _repoPropietario;

        public PropietarioController(IConfiguration configuration)
        {
            _repoPropietario = new RepositorioPropietario(configuration);
        }


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

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Propietario p)
        {
            if (ModelState.IsValid)
            {
                _repoPropietario.Alta(p);
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

        public IActionResult BuscarPropietariosAjax(string q)
        {
            try
            {
                var propietarios = _repoPropietario.ObtenerTodos();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var queryText = q.ToLower();
                    propietarios = propietarios.Where(p =>
                        (p.Nombre != null && p.Nombre.ToLower().Contains(queryText)) ||
                        (p.Apellido != null && p.Apellido.ToLower().Contains(queryText)) ||
                        (p.Dni != null && p.Dni.Contains(queryText))
                    ).ToList();
                }

                var resultados = propietarios.Take(15).Select(p => new
                {
                    id = p.IdPropietario,
                    text = $"{p.Nombre} {p.Apellido} (DNI: {p.Dni})"
                });

                return Json(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}