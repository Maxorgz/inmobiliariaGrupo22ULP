namespace InmobiliariaWeb.Models
{
    public interface IRepositorioPropietario
    {
        IList<Propietario> ObtenerTodos();
        Propietario? ObtenerPorId(int id);
        int Alta(Propietario p);
        int Modificacion(Propietario p);
        int Baja(int id);
    }
}