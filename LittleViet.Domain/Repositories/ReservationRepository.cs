using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

public interface IReservationRepository : IBaseRepository<Reservation>
{
    Task<Reservation> GetById(Guid id);
}
internal class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Reservation> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}