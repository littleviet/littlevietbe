using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

public interface IOrderDetailDomain
{

}
internal class OrderDetailDomain : BaseDomain, IOrderDetailDomain
{
    public OrderDetailDomain(IUnitOfWork uow) : base(uow)
    {
        
    }
}

