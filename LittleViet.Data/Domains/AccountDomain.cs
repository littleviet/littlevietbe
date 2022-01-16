using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LittleViet.Data.Domains;

public interface IAccountDomain
{
    ResponseViewModel Login(string email, string password);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<ResponseViewModel> Create(CreateAccountViewModel createAccountViewModel);
    Task<ResponseViewModel> Update(UpdateAccountViewModel updateAccountViewModel);
    Task<ResponseViewModel> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel);
    Task<BaseListQueryResponseViewModel> GetListAccounts(BaseListQueryParameters parameters);
    Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetAccountById(Guid id);
}
public class AccountDomain : BaseDomain, IAccountDomain
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;


    public AccountDomain(IUnitOfWork uow, IAccountRepository accountRepository, IMapper mapper, IConfiguration configuration) : base(uow)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public ResponseViewModel Login(string email, string password)
    {
        try
        {
            var account = _accountRepository.GetByEmail(email);
            if (account is null || !BCryptNet.Verify(password, account.Password))
            {
                return new ResponseViewModel { Message = "Invalid username or password", Success = false };
            }

            var accountViewModel = _mapper.Map<AccountViewModel>(account);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new (ClaimTypes.Name, accountViewModel.Id.ToString()),
                            new (ClaimTypes.Role, accountViewModel.AccountType.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            accountViewModel.Token = tokenHandler.WriteToken(token);
            return new ResponseViewModel { Payload = accountViewModel, Success = true };

        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Create(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var existedAccount = _accountRepository.GetByEmail(createAccountViewModel.Email);

            if (existedAccount != null)
            {
                return new ResponseViewModel { Success = false, Message = "This email already existed" };
            }
            var account = _mapper.Map<Account>(createAccountViewModel);

            var datetime = DateTime.UtcNow;

            account.Id = Guid.NewGuid();
            account.Password = BCryptNet.HashPassword(createAccountViewModel.Password);
            account.IsDeleted = false;
            account.UpdatedDate = datetime;
            account.CreatedDate = datetime;
            account.UpdatedBy = createAccountViewModel.CreatedBy;

            _accountRepository.Add(account);
            await _uow.SaveAsync();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateAccountViewModel updateAccountViewModel)
    {
        try
        {
            var existedAccount = await _accountRepository.GetById(updateAccountViewModel.Id);

            if (existedAccount != null)
            {
                existedAccount.Lastname = updateAccountViewModel.Lastname;
                existedAccount.Firstname = updateAccountViewModel.Firstname;
                existedAccount.PostalCode = updateAccountViewModel.PostalCode;
                existedAccount.PhoneNumber1 = updateAccountViewModel.PhoneNumber1;
                existedAccount.PhoneNumber2 = updateAccountViewModel.PhoneNumber2;
                existedAccount.Address = updateAccountViewModel.Address;
                existedAccount.AccountType = updateAccountViewModel.AccountType;
                existedAccount.UpdatedDate = DateTime.UtcNow;
                existedAccount.UpdatedBy = existedAccount.UpdatedBy;

                _accountRepository.Modify(existedAccount);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            throw new Exception("This account does not exist");
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel)
    {
        try
        {


            if (updatePasswordViewModel.ConfirmPassword.Equals(updatePasswordViewModel.NewPassword))
            {
                var existedAccount = await _accountRepository.GetById(updatePasswordViewModel.Id);

                if (existedAccount != null)
                {
                    if (!BCryptNet.Verify(updatePasswordViewModel.OldPassword, existedAccount.Password))
                    {
                        return new ResponseViewModel { Message = "Wrong password", Success = false };
                    }

                    existedAccount.Password = BCryptNet.HashPassword(updatePasswordViewModel.NewPassword);

                    _accountRepository.Modify(existedAccount);
                    await _uow.SaveAsync();

                    return new ResponseViewModel { Success = true, Message = "Update successful" };
                }

                return new ResponseViewModel { Success = false, Message = "This account does not exist" };
            }

            return new ResponseViewModel { Success = false, Message = "New password and confirmation password do not match" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var account = await _accountRepository.GetById(id);
            
            if(account != null)
            {
                _accountRepository.Deactivate(account);

                await _uow.SaveAsync();
                return new ResponseViewModel { Message = "Delete successful", Success = true };
            }

            return new ResponseViewModel { Success = false, Message = "This account does not exist" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListAccounts(BaseListQueryParameters parameters)
    {
        try
        {
            var accounts = _accountRepository.DbSet().AsNoTracking();
            var pagingAccounts = await accounts.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync();
            var result = _mapper.Map<List<AccountViewModel>>(pagingAccounts);

            return new BaseListQueryResponseViewModel
            {
                Payload = result,
                Success = true,
                Total = await accounts.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListQueryResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var accounts = _accountRepository.DbSet().AsNoTracking()
                .Where(p => p.Email.ToLower().Contains(keyword) || p.Firstname.ToLower().Contains(keyword) || p.Lastname.ToLower().Contains(keyword));

            var pagingAccounts = await accounts.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync();
            var result = _mapper.Map<List<AccountViewModel>>(pagingAccounts);

            return new BaseListQueryResponseViewModel
            {
                Payload = result,
                Success = true,
                Total = await accounts.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> GetAccountById(Guid id)
    {
        try
        {
            var account = await _accountRepository.GetById(id);

            if (account == null)
            {
                return new ResponseViewModel { Success = false, Message = "This account does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = account };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

