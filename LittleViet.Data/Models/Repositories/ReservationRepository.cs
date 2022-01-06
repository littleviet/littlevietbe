using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

public interface IReservationRepository 
{
    void Create(Reservation reservation);

    void Update(Reservation reservation);

    void DeactivateReservation(Reservation reservation);

    void GetById(Guid id);

}
internal class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(LittleVietContext context) : base(context)
    {

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
        return DbSet().FirstOrDefault<Reservation>(q => q.Id == id);
    }

}