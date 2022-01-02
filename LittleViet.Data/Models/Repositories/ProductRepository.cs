using System.Data.Entity;

namespace LittleViet.Data.Models.Repositories;

public interface IProductRepository
{
    void Create(Product product);
    void Update(Product product);
    void DeactivateProduct(Product product);
    Product GetActiveById(Guid id);
    IQueryable<Product> GetActiveProducs();
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
        Edit(product);
    }

    public void DeactivateProduct(Product product)
    {
        Deactivate(product);
    }

    public Product GetActiveById(Guid id)
    {
        return ActiveOnly().FirstOrDefault(q => q.Id == id);
    }

    public IQueryable<Product> GetActiveProducs()
    {
        return Include(q => q.ProductType).Where(q => q.IsDeleted == false);
    }
}

