using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ServiceEvaObjects
{
    public class ServiceEvaDBCotext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ServiceEvaDB"));
        }
        public virtual DbSet<EvaluationData> EvaluationData { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<ServiceFeedback> ServiceFeedback { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasIndex(x => x.PhoneNumber).IsUnique();
        }
    }
}
