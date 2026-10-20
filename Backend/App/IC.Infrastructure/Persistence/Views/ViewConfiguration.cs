using Microsoft.EntityFrameworkCore;

namespace IC.Infrastructure.Persistence.Views
{
    public static class ViewConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<TeamsAndLeagues>(entity => entity.HasNoKey().ToView("vTeamsAndLeagues"));
            // ...
        }
    }
}
