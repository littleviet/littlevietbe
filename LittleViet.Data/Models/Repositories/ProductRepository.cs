using Microsoft.EntityFrameworkCore;
namespace LittleViet.Data.Models.Repositories;

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
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}

