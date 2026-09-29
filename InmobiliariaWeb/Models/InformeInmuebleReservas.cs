namespace InmobiliariaWeb.Models
{
    public class InformeInmuebleReservas
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; } = "";
        public string PropietarioNombreCompleto { get; set; } = "";
        public int CantidadReservas { get; set; }
    }
}