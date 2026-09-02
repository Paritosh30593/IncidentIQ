using Microsoft.EntityFrameworkCore;

namespace IC.Infrastructure.Persistence.DBContext
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
    }
}

