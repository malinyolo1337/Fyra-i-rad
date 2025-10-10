using Microsoft.EntityFrameworkCore;
using Web.Models; // ändra om dina modeller ligger i annat namespace

namespace Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Spelare> Spelare { get; set; }
        public DbSet<Spel> Spel { get; set; }
        public DbSet<Spelrunda> Spelrundor { get; set; }
        public DbSet<Speldeltagare> Speldeltagare { get; set; }
    }
}
