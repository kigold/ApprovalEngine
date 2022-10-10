using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SampleApp.Core.Data.Entities;

namespace SampleApp.Core.Data
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext()
        {

        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=.;Initial Catalog=approvalEngine;Integrated Security=true;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
