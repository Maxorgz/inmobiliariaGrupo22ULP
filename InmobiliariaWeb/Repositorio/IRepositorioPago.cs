namespace InmobiliariaWeb.Models
{
    public interface IRepositorioPago
    {
        IList<Pago> ObtenerTodos();
        IList<Pago> ObtenerPorReserva(int idReserva);
        Pago? ObtenerPorId(int id);
        int Alta(Pago pago);
        int Modificacion(Pago pago);
        int Anular(int idPago, int idUsuarioAnulador);
    }
}