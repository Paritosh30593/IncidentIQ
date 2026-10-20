using System.Reflection;
using IC.Domain.Entities;
using IC.Infrastructure.Persistence.Functions;
using Microsoft.EntityFrameworkCore;
using IC.Infrastructure.Persistence.Views;

namespace IC.Infrastructure.Persistence.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Test> Tests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            FunctionConfiguration.Configure(modelBuilder);

            ViewConfiguration.Configure(modelBuilder);
        }
    }
}

