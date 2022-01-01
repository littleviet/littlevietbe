namespace LittleViet.Data.Models.Repositories
{
    public interface IProductRepository
    {

    }
    internal class ProductRepository: BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(LittleVietContext context) : base(context)
        {

        }
    }
}
