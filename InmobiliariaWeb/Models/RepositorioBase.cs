using mySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioBase
    {
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            connectionString = configuration.getConnectionString("DefaultConnection") ??"";
        }
    }
}