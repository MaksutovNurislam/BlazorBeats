using Microsoft.EntityFrameworkCore;
using BlazorBeats.Components.Data.Models;
using BlazorBeats.Models;

namespace BlazorBeats.Components.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Beat> Beats => Set<Beat>();
        public DbSet<Order> Orders => Set<Order>();
        
        public DbSet<License> Licenses => Set<License>();
        public DbSet<BeatLicense> BeatLicenses => Set<BeatLicense>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явные имена таблиц
            modelBuilder.Entity<User>().ToTable("user_table");
            modelBuilder.Entity<Profile>().ToTable("profile_table");
            modelBuilder.Entity<Beat>().ToTable("beat_table");
            modelBuilder.Entity<License>().ToTable("license_table");
            modelBuilder.Entity<Order>().ToTable("order_table");
            

            // Модель для связи "многие ко многим"
            modelBuilder.Entity<BeatLicense>()
                .HasKey(bl => new { bl.BeatId, bl.LicenseId });

            modelBuilder.Entity<BeatLicense>()
                .HasOne(bl => bl.Beat)
                .WithMany(b => b.BeatLicenses)
                .HasForeignKey(bl => bl.BeatId);

            modelBuilder.Entity<BeatLicense>()
                .HasOne(bl => bl.License)
                .WithMany(l => l.BeatLicenses)
                .HasForeignKey(bl => bl.LicenseId);

            // Связи в Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(u => u.OrdersAsBuyer)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)
                .WithMany(u => u.OrdersAsSeller)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Beat)
                .WithMany()
                .HasForeignKey(o => o.BeatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.License)
                .WithMany()
                .HasForeignKey(o => o.LicenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
    .HasOne(u => u.Profile)
    .WithOne(p => p.User)
    .HasForeignKey<Profile>(p => p.UserId);


            // Вызов базового метода должен быть последним
            base.OnModelCreating(modelBuilder);
        }
    }
}
