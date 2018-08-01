using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PlasticFreeOcean.Models
{
    
    public class PlasticFreeOceanContext : IdentityDbContext<User,Role,Guid>
    {
        public PlasticFreeOceanContext(DbContextOptions<PlasticFreeOceanContext> options) : base(options)
        { 
        }
        //public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }

        public DbSet<FirebaseNotification> FirebaseNotifications { get; set; }

        // public DbSet<Role> Roles { get; set; }

       
    }
}