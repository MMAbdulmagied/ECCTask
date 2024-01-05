
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DB
{
    public class PortalDBContext : DbContext
    {
        public PortalDBContext()
        {

        }
        public PortalDBContext(DbContextOptions<PortalDBContext> options)
            : base(options)
        {

        }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Complain> Complains { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["PortalCon"].ToString());
                optionsBuilder.EnableSensitiveDataLogging(true);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = -1,
                    Email = "customer@c.com",
                    Mobile = "0511111111",
                    Name = "Default Customer"
                }
            );
        }
    }
}
