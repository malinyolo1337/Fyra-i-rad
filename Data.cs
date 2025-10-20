using Microsoft.EntityFrameworkCore;
using Fyra_i_rad.Models;

namespace Fyra_i_rad.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Spelare> Spelare { get; set; }
        public DbSet<Spel> Spel { get; set; }
        public DbSet<Spelrunda> Spelrunda { get; set; }
        public DbSet<Speldeltagare> Speldeltagare { get; set; }
    }
}

