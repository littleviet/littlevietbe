namespace LittleViet.Data.Models.Repositories
{
    public interface IAccountRepository
    {
        //Account GetAccountByLogin(string email, string password);
        void CreateAccount(Account account);
        void UpdateAccount(Account account);
        void DeactiveAccount(Account account);
        Account GetById(Guid id);
        Account GetByEmail(String email);
    }

    internal class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(LittleVietContext context): base(context)
        {
        }

        //public Account GetAccountByLogin(string email, string password)
        //{
        //    return FirstOrDefaultActive(q => q.Email == email && q.Password == password);
        //}

        public void CreateAccount(Account account)
        {
            Add(account);
        }

        public void UpdateAccount(Account account)
        {
            Edit(account);
        }

        public void DeactiveAccount(Account account)
        {
            Deactivate(account);
        }

        public Account GetById(Guid id)
        {
            return FirstOrDefaultActive(q => q.Id == id);
        }

        public Account GetByEmail(String email)
        {
            return FirstOrDefaultActive(q => q.Email == email);
        }
    }
}
