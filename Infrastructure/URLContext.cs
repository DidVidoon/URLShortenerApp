using Microsoft.EntityFrameworkCore;
using Models;

namespace Infrastructure
{
    public class URLContext : DbContext
    {
        public URLContext(DbContextOptions<URLContext> options) : base(options) 
        { 
            Database.EnsureCreated();
        }

        public DbSet<URL> URLs { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
