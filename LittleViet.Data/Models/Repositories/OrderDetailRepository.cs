namespace LittleViet.Data.Models.Repositories;

public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
{
}
internal class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(LittleVietContext context) : base(context)
    {
    }
}