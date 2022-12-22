using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Repositories;

public interface IVacationRepository : IBaseRepository<Vacation>
{
    Task<Vacation> GetByDateAsync(DateTime date, CancellationToken ct = new());
}

internal class VacationRepository : BaseRepository<Vacation>, IVacationRepository
{
    public VacationRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Vacation> GetByDateAsync(DateTime date, CancellationToken ct = new())
    {
        return DbSet().FirstOrDefaultAsync(q => q.Date == date, ct);
    }
}