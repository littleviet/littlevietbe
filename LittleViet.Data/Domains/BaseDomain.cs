using LittleViet.Data.Repositories;

namespace LittleViet.Data.Domains;

public partial class BaseDomain
{
    protected IUnitOfWork _uow;
    public BaseDomain(IUnitOfWork uow)
    {
        _uow = uow;
    }
}

