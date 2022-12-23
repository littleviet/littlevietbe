using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Domain.Repositories;

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

