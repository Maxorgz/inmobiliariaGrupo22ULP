using System.ComponentModel.DataAnnotations;

namespace InmobiliariaWeb.Models
{
    public enum RolUsuario
    {
        Administrador = 1,
        Empleado = 2
    }

    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un formato de email válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = "";

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int Rol { get; set; } = (int)RolUsuario.Empleado;

        public string RolNombre => Rol == (int)RolUsuario.Administrador ? "Administrador" : "Empleado";
    }
}