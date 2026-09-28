using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public interface IRepositorioImagen
    {
        int Alta(Imagen imagen);
        int Baja(int id);
        Imagen? ObtenerPorId(int id);
        IList<Imagen> BuscarPorInmueble(int idInmueble);
    }
}