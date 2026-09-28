using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaWeb.Models;
using InmobiliariaWeb.Repositorio;

namespace InmobiliariaWeb.Controllers
{
    [Authorize]
    public class ImagenesController : Controller
    {
        private readonly IRepositorioImagen repositorio;
        private readonly IWebHostEnvironment environment;
        private static readonly string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        public ImagenesController(IRepositorioImagen repositorio, IWebHostEnvironment environment)
        {
            this.repositorio = repositorio;
            this.environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes)
        {
            if (imagenes == null || imagenes.Count == 0)
                return BadRequest("No se recibieron archivos.");
                
            if (imagenes.Any(f => !extensionesPermitidas.Contains(Path.GetExtension(f.FileName).ToLowerInvariant())))
                return BadRequest("Las extensiones permitidas son .jpg, .png, .webp y .jpeg");
                
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads", "Inmuebles", id.ToString());
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var file in imagenes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(path, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    Imagen imagen = new Imagen
                    {
                        IdInmueble = id,
                        Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}"
                    };
                    repositorio.Alta(imagen);
                }
            }
            return Ok(repositorio.BuscarPorInmueble(id));
        }

        [HttpPost]
        [Authorize(Policy = "Administrador")] 
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null) return NotFound();

                // Eliminar del servidor el archivo
                string wwwPath = environment.WebRootPath;
                string rutaFisica = Path.Combine(wwwPath, entidad.Url.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                repositorio.Baja(id);
                return Ok(repositorio.BuscarPorInmueble(entidad.IdInmueble));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}