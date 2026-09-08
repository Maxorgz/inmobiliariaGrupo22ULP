namespace InmobiliariaWeb.Models
{
    public interface IRepositorioPropietario
    {
        IList<Propietario> ObtenerTodos();
        Propietario? ObtenerPorId(int id);
        IList<Propietario> BuscarPorNombre(string q);
        int Alta(Propietario p);
        int Modificacion(Propietario p);
        int Baja(int id);
    }
}