using Microsoft.EntityFrameworkCore;

namespace BNRNew_API.Entities
{
    public class MyDBContext : DbContext
    {
        public MyDBContext()
        {
            Database.Migrate();
        }

        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
            Database.Migrate();
        }


        public virtual DbSet<User> user { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseSqlite(@"Data Source=D:\test.db");

            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=mysecretpassword");
        }
    }
}
