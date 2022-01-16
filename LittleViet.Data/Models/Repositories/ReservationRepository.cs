using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

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

    public void Create(Reservation reservation)
    {
        Add(reservation);
    }

    public void Update(Reservation reservation)
    {
        Modify(reservation);
    }

    public void DeactivateReservation(Reservation reservation)
    {
        Deactivate(reservation);
    }

    public Task<Reservation> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync<Reservation>(q => q.Id == id);
    }

}