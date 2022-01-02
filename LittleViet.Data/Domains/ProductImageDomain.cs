using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

    public interface IProductImageDomain
    {

    }
    internal class ProductImageDomain : BaseDomain, IProductImageDomain
    {
        public ProductImageDomain(IUnitOfWork uow) : base(uow)
        {

        }
    }

