namespace LittleViet.Data.Models.Repositories;

    public interface IProductImageRepository
    {

    }
    internal class ProductImageRepository:BaseRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(LittleVietContext context) : base(context)
        {

        }
    }

