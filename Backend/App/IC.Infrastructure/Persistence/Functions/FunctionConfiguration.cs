using Microsoft.EntityFrameworkCore;

namespace IC.Infrastructure.Persistence.Functions
{
    public static class FunctionConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<TopTeam>(entity => entity.HasNoKey().ToFunction("fn_GetTopTeams"));
            // ...
        }
    }
}


