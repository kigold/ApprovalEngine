using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SampleApp.Core.Data
{
    public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SampleDbContext>();
            builder.EnableSensitiveDataLogging(true);
            var connectionString = "Data Source=.;Initial Catalog=approvalEngine;Integrated Security=true;";
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(this.GetType().Assembly.FullName));
            var dbContext = (SampleDbContext)Activator.CreateInstance(typeof(SampleDbContext), builder.Options);
            return dbContext;
        }
    }
}
