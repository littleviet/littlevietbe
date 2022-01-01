namespace LittleViet.Data.Models.Repositories
{
    public interface IReservationRepository
    {

    }
    internal class ReservationRepository:BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(LittleVietContext context) : base(context)
        {

        }
    }
}
