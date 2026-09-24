using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioPago
    {
        IList<Pago> ObtenerTodos(int pagina, int tamanio);
        int ObtenerCantidad();
        IList<Pago> ObtenerPorReserva(int idReserva);
        Pago? ObtenerPorId(int id);
        int Alta(Pago pago);
        int Modificacion(Pago pago);
        int Anular(int idPago, int idUsuarioAnulador);
    }
}