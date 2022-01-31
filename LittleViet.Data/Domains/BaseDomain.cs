using LittleViet.Data.Repositories;

namespace LittleViet.Data.Domains;

public class BaseDomain
{
    protected IUnitOfWork _uow;

    protected BaseDomain(IUnitOfWork uow)
    {
        _uow = uow;
    }
}

