using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

public interface ICouponDomain
{

}
internal class CouponDomain : BaseDomain, ICouponDomain
{
    public CouponDomain(IUnitOfWork uow) : base(uow)
    {

    }
}

