using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PlasticFreeOcean.Models
{
    public class PlasticFreeOceanContext : DbContext
    {
        public PlasticFreeOceanContext(DbContextOptions<PlasticFreeOceanContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(m => m.Id);
             
            // shadow properties
            modelBuilder.Entity<User>().Property<DateTime>("UpdatedTimestamp");

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<User>();

            return base.SaveChanges();
        }

        private void updateUpdatedProperty<T>() where T : class
        {
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}