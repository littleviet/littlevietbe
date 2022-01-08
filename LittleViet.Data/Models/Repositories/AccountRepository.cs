using Microsoft.EntityFrameworkCore;
namespace LittleViet.Data.Models.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    void Create(Account account);
    void Update(Account account);
    void DeactivateAccount(Account account);
    Task<Account> GetById(Guid id);
    Account GetByEmail(String email);
}

internal class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public AccountRepository(LittleVietContext context) : base(context)
    {
    }

    public void Create(Account account)
    {
        Add(account);
    }

    public void Update(Account account)
    {
        base.Modify(account);
    }

    public void DeactivateAccount(Account account)
    {
        Deactivate(account);
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

