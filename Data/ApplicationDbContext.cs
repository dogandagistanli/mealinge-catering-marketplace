using Ceng382_25_26_202311031.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ceng382_25_26_202311031.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<CustomizationOption> CustomizationOptions { get; set; }
        public DbSet<EmailRecord> EmailRecords { get; set; }
        public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MenuItem>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(x => x.UnitPrice)
                .HasPrecision(18, 2);
                builder.Entity<CustomizationOption>()
    .Property(x => x.PriceChange)
    .HasPrecision(18, 2);
    builder.Entity<OrderItem>()
    .Property(x => x.CustomizationPrice)
    .HasPrecision(18, 2);
        }
    }
}