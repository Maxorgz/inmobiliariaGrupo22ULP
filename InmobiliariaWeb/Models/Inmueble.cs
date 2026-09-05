using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "El cupo es obligatorio")]
        [Range(1, 100, ErrorMessage = "El cupo debe ser mayor a 0")]
        public int Cupo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        public int IdTipoInmueble { get; set; }

        public string? TipoInmuebleDescripcion { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitud inválida")]
        public decimal? Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitud inválida")]
        public decimal? Longitud { get; set; }

        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioPorDia { get; set; }

        [Required(ErrorMessage = "El porcentaje de seña es obligatorio")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public decimal PorcentajeReserva { get; set; }

        //public string? ImagenPortada { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        public int IdPropietario { get; set; }

        public string? PropietarioNombreCompleto { get; set; }

        public bool Disponible { get; set; } = true;

    }
}