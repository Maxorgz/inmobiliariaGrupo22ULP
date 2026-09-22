namespace InmobiliariaWeb.Models
{
    public interface IRepositorioUsuario
    {
        IList<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        int Alta(Usuario usuario);
        int Modificacion(Usuario usuario);
        int ModificarPerfil(Usuario usuario);
        int Baja(int id);
    }
}