using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

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
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}