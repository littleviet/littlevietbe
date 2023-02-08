using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product> GetById(Guid id);
}
internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Product> GetById(Guid id)
    {
        return DbSet()
            .Include(p => p.ProductType)
            .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
            .Include(p => p.Servings.Where(s => s.IsDeleted == false))
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}

