using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

public interface IOrderDomain
{

}
internal class OrderDomain : BaseDomain, IOrderDomain
{
    public OrderDomain(IUnitOfWork uow) : base(uow)
    {

    }
}