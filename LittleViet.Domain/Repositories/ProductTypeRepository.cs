using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Repositories;

public interface IProductTypeRepository : IBaseRepository<ProductType>
{

    Task<ProductType> GetById(Guid id);
}
internal class ProductTypeRepository : BaseRepository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<ProductType> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}

