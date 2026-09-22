using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public UsuarioController(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario;
        }

        //gestion usuario

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var usuarios = _repositorioUsuario.ObtenerTodos();
            return View(usuarios);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioUsuario.Alta(usuario);
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var usuario = _repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return RedirectToAction(nameof(Index));
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                usuario.IdUsuario = id;
                _repositorioUsuario.Modificacion(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var usuario = _repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return RedirectToAction(nameof(Index));
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorioUsuario.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Details(int id)
        {
            var usuario = _repositorioUsuario.ObtenerPorId(id);
            if (usuario == null) return RedirectToAction(nameof(Index));
            return View(usuario);
        }

        //autenticacion
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Clave)
        {
            try
            {
                var usuario = _repositorioUsuario.ObtenerPorEmail(Email);

                if (usuario == null || usuario.Clave != Clave)
                {
                    ViewBag.Error = "Email o contraseña incorrectos.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.RolNombre),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }
    }
}