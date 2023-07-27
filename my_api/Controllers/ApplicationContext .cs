using Microsoft.EntityFrameworkCore;

namespace my_api.Controllers
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApplicationContext()
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=mysecretpassword");
        }
    }
}
