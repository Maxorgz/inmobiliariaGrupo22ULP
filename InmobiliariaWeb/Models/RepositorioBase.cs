using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioBase
    {
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            connectionString = "Server=127.0.0.1;Database=inmobiliariagrupo22;Uid=root;Pwd=admin123;";
        }
    }
}