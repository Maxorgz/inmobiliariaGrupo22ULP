using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public class Inquilino
    {
        [Key]
        public int IdInquilino { get; set; }
        
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string Dni { get; set; } = "";
        
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z]{2,15}$", ErrorMessage = "El nombre debe tener entre 2 y 15 caracteres.")]
        public string Nombre { get; set; } = "";
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";
        
        public string? Telefono { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = "";
    }
}