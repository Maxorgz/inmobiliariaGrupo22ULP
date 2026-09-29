using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace InmobiliariaWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public UsuarioController(IRepositorioUsuario repositorioUsuario, IConfiguration config, IWebHostEnvironment environment)
        {
            _repositorioUsuario = repositorioUsuario;
            _config = config;
            _environment = environment;
        }

        private string HashearClave(string clave)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: clave,
                salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"] ?? ""),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        // gestion perfil ---

        [Authorize]
        [HttpGet]
        public IActionResult Perfil()
        {
            var idClaim = User.FindFirst("IdUsuario")?.Value;
            if (idClaim == null) return RedirectToAction("Login");

            int id = int.Parse(idClaim);
            var usuario = _repositorioUsuario.ObtenerPorId(id);
            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(Usuario usuarioModificado)
        {
            try
            {
                var idClaim = User.FindFirst("IdUsuario")?.Value;
                if (idClaim == null) return RedirectToAction("Login");
                int id = int.Parse(idClaim);

                var usuarioActual = _repositorioUsuario.ObtenerPorId(id);
                if (usuarioActual == null) return NotFound();

                usuarioActual.Nombre = usuarioModificado.Nombre;
                usuarioActual.Apellido = usuarioModificado.Apellido;

                if (!string.IsNullOrWhiteSpace(usuarioModificado.ClaveNueva))
                {
                    usuarioActual.Clave = HashearClave(usuarioModificado.ClaveNueva);
                }

                if (usuarioModificado.AvatarFile != null && usuarioModificado.AvatarFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads", "Avatares");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var extension = Path.GetExtension(usuarioModificado.AvatarFile.FileName);
                    var nombreArchivo = $"avatar_{id}_{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(uploadsFolder, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await usuarioModificado.AvatarFile.CopyToAsync(stream);
                    }

                    usuarioActual.Avatar = $"/Uploads/Avatares/{nombreArchivo}";
                }

                _repositorioUsuario.Modificacion(usuarioActual);
                TempData["Mensaje"] = "Perfil actualizado correctamente";
                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuarioModificado);
            }
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
                    usuario.Clave = HashearClave(usuario.Clave); 
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
            usuario.Clave = "";
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

                if (usuario == null || usuario.Clave != HashearClave(Clave ?? ""))
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