using LittleViet.Domain.Models;

namespace LittleViet.Domain.Repositories;

public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
{
}
internal class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(LittleVietContext context) : base(context)
    {
    }
}