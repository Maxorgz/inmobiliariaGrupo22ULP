using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaWeb.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "La reserva es obligatoria")]
        public int IdReserva { get; set; }

        [ForeignKey("IdReserva")]
        public Reserva? Reserva { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        public string Concepto { get; set; } = "";

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        public decimal Importe { get; set; }

        public bool Estado { get; set; } = true;

        public int IdUsuarioCreador { get; set; }
        public int? IdUsuarioAnulador { get; set; }
    }
}