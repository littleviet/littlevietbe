namespace LittleViet.Data.Models.Repositories
{
    public interface ICouponRepository
    {

    }
    internal class CouponRepository: BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(LittleVietContext context) : base(context)
        {
        }
    }
}
