using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SampleApp.Core.Data.Entities;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using Microsoft.EntityFrameworkCore.Design;

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
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalStage> ApprovalStages { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                          .AddEnvironmentVariables()
                         .Build();

            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile("appsettings.Development.json", optional: true)
               .AddJsonFile($"appsettings.{config["ASPNETCORE_ENVIRONMENT"]}.json", optional: true)
               .Build();
            //var connectionString = "Data Source=.;Initial Catalog=approvalEngine;Integrated Security=true;";
            var connectionString = configuration["ConnectionStrings:Default"];
            optionsBuilder.UseSqlServer(connectionString);
        }

        public void SeedData(ModelBuilder builder)
        {
            var stages = new List<ApprovalStage>()
            {
                new ApprovalStage()
                {
                    Id = 1,
                    ApprovalType = ApprovalEngine.Enums.ApprovalType.StudentUser,
                    Name = "HOD",
                    StageOrder = 1,
                    Permission = ApprovalEngine.Enums.Permission.HOD,
                    DeclineToOrder = 1,
                    Version = 1
                },
                new ApprovalStage()
                {
                    Id = 2,
                    ApprovalType = ApprovalEngine.Enums.ApprovalType.StudentUser,
                    Name = "Admin",
                    StageOrder = 1,
                    Permission = ApprovalEngine.Enums.Permission.Admin,
                    DeclineToOrder = 1,
                    Version = 1
                },
                new ApprovalStage()
                {
                    Id = 3,
                    ApprovalType = ApprovalEngine.Enums.ApprovalType.StudentUser,
                    Name = "IT",
                    StageOrder = 1,
                    Permission = ApprovalEngine.Enums.Permission.IT,
                    DeclineToOrder = 2,
                    Version = 1
                }
            };

            builder.Entity<ApprovalStage>().HasData(stages);
        }
    }
}
