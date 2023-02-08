using LittleViet.Data.Domains.ProductType;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models;

public class LittleVietContext : DbContext
{
    public LittleVietContext(DbContextOptions<LittleVietContext> options)
        : base(options)
    {
    }

    internal DbSet<Account> Account { get; set; }
    internal DbSet<Coupon> Coupon { get; set; }
    internal DbSet<CouponType> CouponType { get; set; }
    internal DbSet<Order> Order { get; set; }
    internal DbSet<OrderDetail> OrderDetail { get; set; }
    internal DbSet<ProductImage> ProductImage { get; set; }
    internal DbSet<Product> Product { get; set; }
    internal DbSet<Reservation> Reservation { get; set; }
    internal DbSet<Serving> Serving { get; set; }
    internal DbSet<ProductType> ProductType { get; set; }
    internal DbSet<Vacation> Vacations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Coupon>()
            .HasIndex(c => c.CouponCode)
            .IsUnique();

        builder.Entity<ProductType>()
            .HasData(new ProductType()
            {
                Id = Constants.PackagedProductTypeId,
                Description = "Packaged products",
                Name = "Packaged Products",
                CaName = "Productes envasats",
                EsName = "Productos Empaquetados",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            });
        
        // builder.Entity<Account>() //TODO: seed this
        //     .HasData(new Account()
        //     {
        //         Id = new Guid("ce635d8c-6c16-4175-8a8d-8c9ad3b94da5"),
        //         AccountType = RoleEnum.ADMIN,
        //         Email = "eslittleviet@gmail.com",
        //         CreatedDate = DateTime.UtcNow,
        //         UpdatedDate = DateTime.UtcNow,
        //     });
    }
}