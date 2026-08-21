namespace InmobiliariaWeb.Models
{
    public interface IRepositorioInquilino
    {
        IList<Inquilino> ObtenerTodos();
        Inquilino? ObtenerPorId(int id);
        int Alta(Inquilino i);
        int Modificacion(Inquilino i);
        int Baja(int id);
    }
}