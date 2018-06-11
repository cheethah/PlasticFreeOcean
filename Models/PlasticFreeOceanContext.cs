using Microsoft.EntityFrameworkCore;

namespace PlasticFreeOcean.Models
{
    public class PlasticFreeOceanContext : DbContext
    {
        public PlasticFreeOceanContext(DbContextOptions<PlasticFreeOceanContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}