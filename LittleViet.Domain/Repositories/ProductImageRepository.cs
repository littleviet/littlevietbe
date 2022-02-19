using LittleViet.Data.Models;

namespace LittleViet.Data.Repositories;

public interface IProductImageRepository : IBaseRepository<ProductImage>
{

}
internal class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(LittleVietContext context) : base(context)
    {
    }
}