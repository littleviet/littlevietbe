namespace LittleViet.Data.Models.Repositories
{
    public interface IServingRepository
    {

    }
    internal class ServingRepository:BaseRepository<Serving>, IServingRepository
    {
        public ServingRepository(LittleVietContext context) : base(context)
        {

        }
    }
}
