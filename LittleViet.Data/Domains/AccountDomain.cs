using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrcyptNet = BCrypt.Net.BCrypt;

namespace LittleViet.Data.Domains
{
    public interface IAccountDomain
    {
        ResponseVM Login(string email, string password);
        ResponseVM Deactivate(Guid id);
        ResponseVM Create(AccountVM accountVM);
        ResponseVM Update(AccountVM accountVM);
    }
    public class AccountDomain : BaseDomain, IAccountDomain
    {
        private IAccountRepository _accRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public AccountDomain(IUnitOfWork uow, IAccountRepository accountRepository, IMapper mapper, IConfiguration configuration) : base(uow)
        {
            _accRepo = accountRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public ResponseVM Login(string email, string password)
        {
            try
            {
                var account = _accRepo.GetByEmail(email);
                if (account is null || !BrcyptNet.Verify(password, account.Password))
                {
                    return new ResponseVM { Message = "Invalid username or password", Success = false };
                }

                var accVM = _mapper.Map<AccountVM>(account);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, accVM.Id.ToString()),
                    new Claim(ClaimTypes.Role, accVM.AccountType.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                accVM.Token = tokenHandler.WriteToken(token);
                return new ResponseVM { Data = accVM, Success = true };

            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Create(AccountVM accountVM)
        {
            try
            {
                var existedAccount = _accRepo.GetByEmail(accountVM.Email);

                if (existedAccount != null)
                {
                    return new ResponseVM { Success = false, Message = "This email already existed" };
                }
                var account = _mapper.Map<Account>(accountVM);

                var datetime = DateTime.UtcNow;

                account.Id = Guid.NewGuid();
                account.Password = BrcyptNet.HashPassword(accountVM.Password);
                account.IsDeleted = false;
                account.UpdatedDate = datetime;
                account.CreatedDate = datetime;

                _accRepo.Create(account);
                _uow.Save();

                return new ResponseVM { Success = true, Message = "Create successful" };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Update(AccountVM accountVM)
        {
            try
            {
                var account = _mapper.Map<Account>(accountVM);

                var datetime = DateTime.UtcNow;

                account.UpdatedDate = datetime;
                account.CreatedDate = datetime;

                _accRepo.Update(account);
                _uow.Save();

                return new ResponseVM { Success = true, Message = "Update successful" };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }

        public ResponseVM Deactivate(Guid id)
        {
            try
            {
                var account = _accRepo.GetById(id);
                _accRepo.DeactivateAccount(account);

                _uow.Save();
                return new ResponseVM { Message = "Delete successful", Success = true };
            }
            catch (Exception e)
            {
                return new ResponseVM { Success = false, Message = e.Message };
            }
        }
    }
}
