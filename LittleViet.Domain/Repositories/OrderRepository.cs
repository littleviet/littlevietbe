using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order> GetById(Guid id);
}
internal class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Order> GetById(Guid id)
    {
        return DbSet()
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Serving)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}