namespace LittleViet.Data.Models.Repositories;

public interface IAccountRepository
{
    void Create(Account account);
    void Update(Account account);
    void DeactivateAccount(Account account);
    Account GetActiveById(Guid id);
    Account GetActiveByEmail(String email);
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

    public Account GetActiveById(Guid id)
    {
        return DbSet().FirstOrDefault(q => q.Id == id);
    }

    public Account GetActiveByEmail(String email)
    {
        return DbSet().FirstOrDefault(q => q.Email == email);
    }
}

