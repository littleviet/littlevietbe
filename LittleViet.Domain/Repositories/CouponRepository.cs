using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

public interface ICouponRepository : IBaseRepository<Coupon>
{
    Task<Coupon> GetById(Guid id);
}
internal class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Coupon> GetById(Guid id)
    {
        return DbSet()
            .Include(q => q.CouponType)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}