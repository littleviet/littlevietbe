using Microsoft.EntityFrameworkCore;
namespace LittleViet.Data.Models.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account> GetById(Guid id);
    Account GetByEmail(String email);
}

internal class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public AccountRepository(LittleVietContext context) : base(context)
    {
    }

    public Task<Account> GetById(Guid id)
    {
        return DbSet().FirstOrDefaultAsync(q => q.Id == id);
    }

    public Account GetByEmail(String email)
    {
        return DbSet().FirstOrDefault(q => q.Email == email);
    }
}

