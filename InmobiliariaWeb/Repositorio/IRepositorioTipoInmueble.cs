using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioTipoInmueble
    {
        IList<TipoInmueble> ObtenerLista(int pagina, int tamano);
        IList<TipoInmueble> ObtenerTodos();
        int ObtenerCantidad();
        TipoInmueble? ObtenerPorId(int id);
        IList<TipoInmueble> BuscarPorDescripcion(string q);
        int Alta(TipoInmueble t);
        int Modificacion(TipoInmueble t);
    }
}