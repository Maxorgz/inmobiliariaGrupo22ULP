namespace InmobiliariaWeb.Models
{
    public interface IRepositorioInmueble
    {
        IList<Inmueble> ObtenerLista(int pagina, int tamano);
        int ObtenerCantidad();
        Inmueble? ObtenerPorId(int id);
        IList<Inmueble> BuscarPorDireccion(string q);
        IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime desde, DateTime hasta);
        int Alta(Inmueble i);
        int Modificacion(Inmueble i);
        int CambiarDisponibilidad(int id, bool disponible);
    }
}