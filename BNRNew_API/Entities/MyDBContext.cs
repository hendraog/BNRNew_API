using BNRNew_API.config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;

namespace BNRNew_API.Entities
{
    public class MyDBContext : DbContext
    {
        public MyDBContext()
        {
            Database.Migrate();
        }

        public  MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
            Database.Migrate();

            var userData = user.FirstOrDefault();
            if (userData == null)
            {
                var s = new User
                {
                    UserName = "admin",
                    Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    Active = true,
                    Role = AppConstant.Role_SUPERADMIN,
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow
                };

                this.user.Add(s);
            }
            this.SaveChanges();
        }


        public virtual DbSet<User> user { get; set; }
        public virtual DbSet<Golongan> golongan { get; set; }
        public virtual DbSet<GolonganPlat> golonganPlat { get; set; }
        public virtual DbSet<CargoManifest> CargoManifests { get; set; }
        public virtual DbSet<CargoDetail> CargoDetails { get; set; }
        public virtual DbSet<Ticket> ticket { get; set; }
        public virtual DbSet<Kapal> kapal { get; set; }
        public virtual DbSet<Sequence> sequence { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CargoManifest>().HasMany(g => g.detailData).WithOne(e => e.cargoManifest).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CargoManifest>().HasOne(g => g.CreatedByData).WithMany(e => e.cargoManifests).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<CargoManifest>().HasOne(e => e.UpdatedByData).WithMany(e => e.cargoManifests).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoDetail>().HasOne(g => g.ticketData).WithOne(e => e.cargoDetailData).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>().HasOne(e=> e.golonganData).WithMany(e => e.tickets).HasForeignKey(e => e.golongan).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Ticket>().HasOne(e => e.cargoDetailData).WithOne(e => e.ticketData).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Ticket>().HasOne(e => e.CreatedByData).WithMany(e => e.tickets).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Ticket>().HasOne(e => e.UpdatedByData).WithMany(e => e.tickets).OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<Golongan>().HasMany(e => e.tickets).WithOne(e => e.golonganData).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Golongan>().HasMany(e => e.golonganPlat).WithOne(e => e.golongan).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Golongan>().HasOne(e => e.CreatedByData).WithMany(e => e.golongans).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Golongan>().HasOne(e => e.UpdatedByData).WithMany(e => e.golongans).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GolonganPlat>().HasOne(e => e.golongan).WithMany(e => e.golonganPlat).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GolonganPlat>().HasOne(e => e.CreatedByData).WithMany(e => e.golonganPlats).OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<GolonganPlat>().HasOne(e => e.UpdatedByData).WithMany(e => e.golonganPlats).OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(true);
            //optionsBuilder.UseSqlite(@"Data Source=D:\test.db");

            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=mysecretpassword");
        }
    }
}
