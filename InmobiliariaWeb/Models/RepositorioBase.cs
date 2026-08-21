using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioBase
    {
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") ??"";
        }
    }
}