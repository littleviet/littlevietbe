using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Models.Repositories;

public interface IServingRepository : IBaseRepository<Serving>
{
    Task<Serving> GetById(Guid id);
}
internal class ServingRepository : BaseRepository<Serving>, IServingRepository
{
    public ServingRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Serving> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }
}