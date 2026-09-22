using Microsoft.Extensions.Configuration;

namespace InmobiliariaWeb.Repositorio
{
    public abstract class RepositorioBase
    {
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
    }
}