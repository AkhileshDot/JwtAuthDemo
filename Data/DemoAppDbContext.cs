using Microsoft.EntityFrameworkCore;


namespace JwtAuthDemo.Data
{
    public class DemoAppDbContext : DbContext
    {
        public DemoAppDbContext(DbContextOptions<DemoAppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SeederLog> SeederLogs { get; set; }
    }
}
