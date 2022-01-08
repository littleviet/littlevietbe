using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    void Create(Product product);
    void Update(Product product);
    void DeactivateProduct(Product product);
    Task<Product> GetById(Guid id);
}
internal class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(LittleVietContext context) : base(context)
    {
    }
    public void Create(Product product)
    {
        Add(product);
    }

    public void Update(Product product)
    {
        Modify(product);
    }

    public void DeactivateProduct(Product product)
    {
        Deactivate(product);
    }

    public Task<Product> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}

