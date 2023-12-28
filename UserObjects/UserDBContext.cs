using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UserObjects
{
    public class UserDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("UserDB"));
        }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<RoleDistribution> RoleDistribution { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<VerificationCode> VerificationCode { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email).IsUnique();
        }
    }
}
