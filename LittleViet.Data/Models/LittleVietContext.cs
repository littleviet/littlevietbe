using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models
{
    public class LittleVietContext : DbContext
    {
        public LittleVietContext(DbContextOptions<LittleVietContext> options)
            : base(options)
        {
        }

        internal DbSet<Account> Account { get; set; }
        internal DbSet<Coupon> Coupon { get; set; }
        internal DbSet<Order> Order { get; set; }
        internal DbSet<OrderDetail> OrderDetail { get; set; }
        internal DbSet<ProductImage> ProductImage { get; set; }
        internal DbSet<Product> Product { get; set; }
        internal DbSet<Reservation> Reservation { get; set; }
        internal DbSet<Serving> Serving { get; set; }
        internal DbSet<ProductType> ProductType { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=littlevietdev.postgres.database.azure.com; Port=5432; Username=littleviet; Password=natteam@2021; Database=LittleViet");
    }
}
