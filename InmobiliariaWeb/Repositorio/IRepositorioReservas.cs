namespace InmobiliariaWeb.Models
{
    public interface IRepositorioReserva
    {
        IList<Reserva> ObtenerLista(int pagina, int tamano);
        int ObtenerCantidad();
        Reserva? ObtenerPorId(int id);
        bool ExisteSolapamiento(int idInmueble, DateTime desde, DateTime hasta, int idReservaExcluir);
        int Alta(Reserva r);
        int Modificacion(Reserva r);
    }
}