using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

    public partial class BaseDomain
    {
        protected IUnitOfWork _uow;
        public BaseDomain(IUnitOfWork uow)
        {
            _uow = uow;
        }
    }

