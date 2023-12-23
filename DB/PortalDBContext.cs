
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DB
{
    public class PortalDBContext: DbContext
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
                optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["RMSConnectionString"].ToString());
                optionsBuilder.EnableSensitiveDataLogging(true);
            }
        }
    }
}
