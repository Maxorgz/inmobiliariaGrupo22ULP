using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public class TipoInmueble
    {
        public int IdTipoInmueble { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(50, ErrorMessage = "La descripción no puede superar los 50 caracteres")]
        public string Descripcion { get; set; } = "";

    }
}