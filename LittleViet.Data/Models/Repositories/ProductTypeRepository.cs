namespace LittleViet.Data.Models.Repositories;

    public interface IProductTypeRepository
    {
        void Create(ProductType productType);
        void Update(ProductType productType);
        void DeactivateProductType(ProductType productType);
        ProductType GetActiveById(Guid id);
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
            Edit(productType);
        }

        public void DeactivateProductType(ProductType productType)
        {
            Deactivate(productType);
        }

        public ProductType GetActiveById(Guid id)
        {
            return ActiveOnly().FirstOrDefault(q => q.Id == id);
        }

        public IQueryable<ProductType> GetActiveProductTypes()
        {
            return ActiveOnly();
        }
    }

