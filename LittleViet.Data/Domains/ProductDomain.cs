using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains
{
    public interface IProductDomain
    {

    }
    internal class ProductDomain : BaseDomain, IProductDomain
    {
        public ProductDomain(IUnitOfWork uow): base(uow)
        {

        }
    }
}
