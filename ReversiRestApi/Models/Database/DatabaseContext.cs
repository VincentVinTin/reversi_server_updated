using Microsoft.EntityFrameworkCore;

namespace ReversiRestApi.Models.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<SpelDatabase> Spellen { get; set; }
    }
}
