using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioPropietario
    {
        IList<Propietario> ObtenerTodos(int pagina, int tamanio);
        IList<Propietario> ObtenerTodos();
        int ObtenerTotal();
        Propietario? ObtenerPorId(int id);
        IList<Propietario> BuscarPorNombre(string q);
        int Alta(Propietario p);
        int Modificacion(Propietario p);
        int Baja(int id);
    }
}