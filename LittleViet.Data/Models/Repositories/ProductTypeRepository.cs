namespace LittleViet.Data.Models.Repositories
{
    public interface IProductTypeRepository
    {

    }
    internal class ProductTypeRepository:BaseRepository<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(LittleVietContext context): base(context)
        {

        }
    }
}
