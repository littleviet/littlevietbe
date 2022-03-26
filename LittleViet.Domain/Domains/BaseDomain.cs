using LittleViet.Data.Repositories;

namespace LittleViet.Data.Domains;

public class BaseDomain
{
    protected readonly IUnitOfWork _uow;

    protected BaseDomain(IUnitOfWork uow)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }
}

