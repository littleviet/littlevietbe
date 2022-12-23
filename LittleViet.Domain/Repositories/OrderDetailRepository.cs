using LittleViet.Data.Models;

namespace LittleViet.Data.Repositories;

public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
{
}
internal class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(LittleVietContext context) : base(context)
    {
    }
}