using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z]{2,15}$", ErrorMessage = "El nombre debe tener entre 2 y 15 caracteres.")]
        public string Nombre { get; set; } = "";
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [RegularExpression(@"^[A-Za-z]{2,15}$", ErrorMessage = "El apellido debe tener entre 2 y 15 caracteres.")]
        public string Apellido { get; set; } = "";
        
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 digitos.")]
        public string Dni { get; set; } = "";
        
        
        [RegularExpression(@"^(?=.*[0-9]).{10}$", ErrorMessage = "El telefono debe tener 10 digitos.")]
        public string? Telefono { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = "";
        
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,50}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, incluyendo al menos una letra y un num.")]
        public string Clave { get; set; } = "";

        public bool IsActive{ get; set; } = true;
    }
}