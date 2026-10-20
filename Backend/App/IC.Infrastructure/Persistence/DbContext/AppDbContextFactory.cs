using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IC.Infrastructure.Persistence.DBContext
{
    // Used only by `dotnet ef` tooling (design time), so CI can generate migrations
    // and scripts without starting the WebAPI or needing a real database.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string connection = Environment.GetEnvironmentVariable("ConnectionStrings__IC")
                ?? "Server=(localdb)\\mssqllocaldb;Database=IncidentIQ;Trusted_Connection=True;MultipleActiveResultSets=true";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connection)
                .Options;

            return new AppDbContext(options);
        }
    }
}
