using LittleViet.Domain.Models;

namespace LittleViet.Domain.Repositories;

public interface IProductImageRepository : IBaseRepository<ProductImage>
{

}
internal class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(LittleVietContext context) : base(context)
    {
    }
}