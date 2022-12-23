using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

public interface IVacationRepository : IBaseRepository<Vacation>
{
    Task<Vacation> GetByTimeAsync(DateTime date, CancellationToken ct = new());
}

internal class VacationRepository : BaseRepository<Vacation>, IVacationRepository
{
    public VacationRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Vacation> GetByTimeAsync(DateTime date, CancellationToken ct = new())
    {
        return DbSet()
            .FirstOrDefaultAsync(
                q => q.Date == date.Date 
                     && ((q.To == null && q.From == null) || (q.To >= date && q.From <= date)), ct);
    }
}