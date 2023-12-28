using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SystemObjects
{
    public class SystemDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SystemDB"));
        }
        public virtual DbSet<EvaluationCriteria> EvaluationCriteria { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<SystemInformation> SystemInformation { get; set; }
        public virtual DbSet<SystemLog> SystemLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>()
                .HasIndex(x => x.SerName).IsUnique();

            modelBuilder.Entity<EvaluationCriteria>()
                .HasIndex(x => x.CriDesc).IsUnique();
        }
    }
}
