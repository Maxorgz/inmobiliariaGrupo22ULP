using Microsoft.EntityFrameworkCore;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
    }
}