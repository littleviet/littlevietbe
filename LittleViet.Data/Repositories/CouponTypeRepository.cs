using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Repositories;

public interface ICouponTypeRepository : IBaseRepository<CouponType>
{

    Task<CouponType> GetById(Guid id);
}
internal class CouponTypeRepository : BaseRepository<CouponType>, ICouponTypeRepository
{
    public CouponTypeRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<CouponType> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}

