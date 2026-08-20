using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";
        
        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string Dni { get; set; } = "";
        
        public string? Telefono { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = "";
        
        public string? Clave { get; set; }
    }
}