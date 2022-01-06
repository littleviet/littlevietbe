using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

public interface IReservationDomain
{

}
internal class ReservationDomain : BaseDomain, IReservationDomain
{
    public ReservationDomain(IUnitOfWork uow) : base(uow)
    {

    }
}

