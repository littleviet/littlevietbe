using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

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

