using BNRNew_API.config;
using Microsoft.AspNetCore.Identity;
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

            var userData = user.FirstOrDefault();
            if (userData == null)
            {
                this.user.Add(new User
                {
                    UserName = "admin",
                    Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    Active = true,
                    Role = AppConstant.Role_SUPERADMIN,
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow
                });
            }
            this.SaveChanges();
        }


        public virtual DbSet<User> user { get; set; }
        public virtual DbSet<Golongan> golongan { get; set; }
        public virtual DbSet<GolonganPlat> golonganPlat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(true);
            //optionsBuilder.UseSqlite(@"Data Source=D:\test.db");

            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=mysecretpassword");
        }
    }
}
