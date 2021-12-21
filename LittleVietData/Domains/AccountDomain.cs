using LittleVietData.Models.Global;
using LittleVietData.Models.Repositories;
using LittleVietData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleVietData.Domains
{
    public interface IAccountDomain
    {
        ResponseViewModel Login(string email, string password);
    }
    public class AccountDomain : BaseDomain, IAccountDomain
    {
        public AccountDomain(IUnitOfWork uow): base(uow)
        {

        }

        public ResponseViewModel Login(string email, string password)
        {
            try
            {
                var accRepo = _uow.GetService<IAccountRepository>();

                var account = accRepo.GetAccountByLogin(email, password);
                if(account is null)
                {
                    return new ResponseViewModel { Message = "Invalid username or password", Success = false };
                }
                return new ResponseViewModel { Data = account, Success = true };

            }
            catch(Exception e)
            {
                return new ResponseViewModel { Success = false, Message = e.Message };
            }
        }
    }
}
