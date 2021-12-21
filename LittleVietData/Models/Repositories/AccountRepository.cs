using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Models.Repositories
{
    public interface IAccountRepository
    {
        Account GetAccountByLogin(string email, string password);
    }

    internal class AccountRepository : IAccountRepository
    {
        List<Account> accList;

        public AccountRepository()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/";
            var data = File.ReadAllText(@"D:\Project\LittleVietBE\LittleVietBE\LittleVietData\TestData\AccountData.json");
            accList = JsonConvert.DeserializeObject<List<Account>>(data) ?? new List<Account>();
        }

        public Account GetAccountByLogin(string email, string password)
        {
            return accList.FirstOrDefault(q => q.Email == email && q.Password == password);
        }
    }
}
