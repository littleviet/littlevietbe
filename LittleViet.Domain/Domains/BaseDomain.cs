using LittleViet.Domain.Repositories;

namespace LittleViet.Domain.Domains;

public class BaseDomain
{
    protected readonly IUnitOfWork _uow;

    protected BaseDomain(IUnitOfWork uow)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }
}

