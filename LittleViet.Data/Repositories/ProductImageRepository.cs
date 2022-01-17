using LittleViet.Data.Models;

namespace LittleViet.Data.Repositories;

public interface IProductImageRepository
{

}
internal class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(LittleVietContext context) : base(context)
    {
    }
}