using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioInmueble
    {
        IList<Inmueble> ObtenerLista(int pagina, int tamano);
        IList<Inmueble> ObtenerTodos();
        int ObtenerCantidad();
        Inmueble? ObtenerPorId(int id);
        IList<Inmueble> BuscarPorDireccion(string q);
        IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime desde, DateTime hasta);
        int Alta(Inmueble i);
        int Modificacion(Inmueble i);
        int CambiarDisponibilidad(int id, bool disponible);
        IList<Inmueble> ObtenerTodosConDetalle(bool? disponible);
        IList<Inmueble> ObtenerPorPropietario(int idPropietario);
        IList<InformeInmuebleReservas> MasReservados(int dias);
        IList<Inmueble> SinReservas(int dias);
    }
}