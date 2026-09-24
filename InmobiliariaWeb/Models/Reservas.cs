using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaWeb.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        public int IdInmueble { get; set; }

        [ForeignKey("IdInquilino")]
        public Inquilino? Inquilino { get; set; }

        [ForeignKey("IdInmueble")]
        public Inmueble? Inmueble { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaHastaOriginal { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaTerminacionAnticipada { get; set; }

        public decimal? Multa { get; set; }

        public decimal MontoPorDia { get; set; }

        [NotMapped]
        public bool EstaVigente =>
            !FechaTerminacionAnticipada.HasValue &&
            DateTime.Today >= FechaDesde &&
            DateTime.Today <= FechaHasta;
    }
}