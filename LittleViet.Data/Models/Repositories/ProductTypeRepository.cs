using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

public interface IProductTypeRepository : IBaseRepository<ProductType>
{
    void Create(ProductType productType);
    void Update(ProductType productType);
    void DeactivateProductType(ProductType productType);
    Task<ProductType> GetById(Guid id);
}
internal class ProductTypeRepository : BaseRepository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(LittleVietContext context) : base(context)
    {

    }

    public void Create(ProductType productType)
    {
        Add(productType);
    }

    public void Update(ProductType productType)
    {
        Modify(productType);
    }

    public void DeactivateProductType(ProductType productType)
    {
        Deactivate(productType);
    }

    public Task<ProductType> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}

