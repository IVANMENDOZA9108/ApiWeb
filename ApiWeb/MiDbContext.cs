using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace ApiWeb
{
    public class MiDbContext : DbContext
    {

        public MiDbContext(DbContextOptions<MiDbContext> options) : base(options)
        {
        }

        
        public DbSet<Usuario>? Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-50SHFS4;Database=IMENDOZA;User Id=sa;Password=Iemr9108;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
