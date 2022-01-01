namespace LittleViet.Data.Models.Repositories
{
    public interface IOrderRepository
    {

    }
    internal class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(LittleVietContext context) : base(context)
        {

        }
    }
}
