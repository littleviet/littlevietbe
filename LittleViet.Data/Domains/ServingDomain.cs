using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains
{
    public interface IServingDomain
    {

    }
    internal class ServingDomain : BaseDomain, IServingDomain
    {
        public ServingDomain(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
