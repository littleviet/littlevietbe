using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Repositories;

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
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}