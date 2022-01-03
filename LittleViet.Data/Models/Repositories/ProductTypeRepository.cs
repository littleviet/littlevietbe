namespace LittleViet.Data.Models.Repositories;

public interface IProductTypeRepository : IBaseRepository<ProductType>
{
    void Create(ProductType productType);
    void Update(ProductType productType);
    void DeactivateProductType(ProductType productType);
    ProductType GetById(Guid id);
    IQueryable<ProductType> GetProductType();
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

    public ProductType GetById(Guid id)
    {
        return DbSet().FirstOrDefault(q => q.Id == id);
    }

    public IQueryable<ProductType> GetProductType()
    {
        return DbSet();
    }
}

