namespace InmobiliariaWeb.Models
{
    public interface IRepositorioTipoInmueble
    {
        IList<TipoInmueble> ObtenerLista(int pagina, int tamano);
        int ObtenerCantidad();
        TipoInmueble? ObtenerPorId(int id);
        IList<TipoInmueble> BuscarPorDescripcion(string q);
        int Alta(TipoInmueble t);
        int Modificacion(TipoInmueble t);
    }
}