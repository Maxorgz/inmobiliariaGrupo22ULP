using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioInquilino
    {
        IList<Inquilino> ObtenerLista(int pagina, int tamanio);
        IList<Inquilino> ObtenerTodos();
        int ObtenerTotal();
        Inquilino? ObtenerPorId(int id);
        IList<Inquilino> BuscarPorNombre(string q); 
        int Alta(Inquilino i);
        int Modificacion(Inquilino i);
        int Baja(int id);
    }
}