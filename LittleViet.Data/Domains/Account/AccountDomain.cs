using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static LittleViet.Infrastructure.EntityFramework.SqlHelper;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LittleViet.Data.Domains.Account;

public interface IAccountDomain
{
    ResponseViewModel Login(string email, string password);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<ResponseViewModel> Create(Guid userId, CreateAccountViewModel createAccountViewModel);
    Task<ResponseViewModel> Register(CreateAccountViewModel createAccountViewModel);
    Task<ResponseViewModel> Update(UpdateAccountViewModel updateAccountViewModel);
    Task<ResponseViewModel> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel);
    Task<BaseListResponseViewModel> GetListAccounts(GetListAccountParameters parameters);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
    Task<ResponseViewModel> GetAccountById(Guid id);
}

public class AccountDomain : BaseDomain, IAccountDomain
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;


    public AccountDomain(IUnitOfWork uow, IAccountRepository accountRepository, IMapper mapper,
        IConfiguration configuration) : base(uow)
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
                return new ResponseViewModel {Message = "Invalid username or password", Success = false};
            }

            var accountViewModel = _mapper.Map<GenericAccountViewModel>(account);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:JwtSecret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, accountViewModel.Id.ToString()),
                    new(ClaimTypes.Role, accountViewModel.AccountType.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            accountViewModel.Token = tokenHandler.WriteToken(token);
            return new ResponseViewModel {Payload = accountViewModel, Success = true};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Create(Guid userId, CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var existedAccount = _accountRepository.GetByEmail(createAccountViewModel.Email);

            if (existedAccount != null)
            {
                return new ResponseViewModel {Success = false, Message = "This email already existed"};
            }

            var account = _mapper.Map<Models.Account>(createAccountViewModel);

            account.Id = Guid.NewGuid();
            account.Password = BCryptNet.HashPassword(createAccountViewModel.Password);

            _accountRepository.Add(account);
            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true, Message = "Create successful"};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Register(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var existedAccount = _accountRepository.GetByEmail(createAccountViewModel.Email);

            if (existedAccount != null)
            {
                return new ResponseViewModel {Success = false, Message = "This email already existed"};
            }

            var account = _mapper.Map<Models.Account>(createAccountViewModel);

            account.Id = Guid.NewGuid();
            account.Password = BCryptNet.HashPassword(createAccountViewModel.Password);

            _accountRepository.Add(account);
            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true, Message = "Create successful"};
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

                _accountRepository.Modify(existedAccount);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Update successful"};
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
                        return new ResponseViewModel {Message = "Wrong password", Success = false};
                    }

                    existedAccount.Password = BCryptNet.HashPassword(updatePasswordViewModel.NewPassword);

                    _accountRepository.Modify(existedAccount);
                    await _uow.SaveAsync();

                    return new ResponseViewModel {Success = true, Message = "Update successful"};
                }

                return new ResponseViewModel {Success = false, Message = "This account does not exist"};
            }

            return new ResponseViewModel
                {Success = false, Message = "New password and confirmation password do not match"};
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

            if (account == null)
                return new ResponseViewModel {Success = false, Message = "This account does not exist"};

            _accountRepository.Deactivate(account);

            await _uow.SaveAsync();
            return new ResponseViewModel {Message = "Delete successful", Success = true};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel> GetListAccounts(GetListAccountParameters parameters)
    {
        try
        {
            var accounts = _accountRepository.DbSet().AsNoTracking()
                .ApplySort(parameters.OrderBy)
                .WhereIf(!string.IsNullOrEmpty(parameters.Email),
                    ContainsIgnoreCase<Models.Account>(nameof(Models.Account.Email), parameters.Email))
                .WhereIf(!string.IsNullOrEmpty(parameters.FullName),
                    ContainsIgnoreCase<Models.Account>(
                        new[] {nameof(Models.Account.Firstname), nameof(Models.Account.Lastname)},
                        parameters.FullName))
                .WhereIf(!string.IsNullOrEmpty(parameters.Address),
                    ContainsIgnoreCase<Models.Account>(nameof(Models.Account.Address), parameters.Address))
                .WhereIf(!string.IsNullOrEmpty(parameters.PhoneNumber),
                    ContainsIgnoreCase<Models.Account>(
                        new[] {nameof(Models.Account.PhoneNumber1), nameof(Models.Account.PhoneNumber2)},
                        parameters.Address))
                .WhereIf(!string.IsNullOrEmpty(parameters.PostalCode),
                    a => a.PostalCode.Contains(parameters.PostalCode));

            return new BaseListResponseViewModel
            {
                Payload = await accounts
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new GenericAccountViewModel()
                    {
                        AccountType = q.AccountType,
                        Address = q.Address,
                        Email = q.Email,
                        Firstname = q.Firstname,
                        Id = q.Id,
                        Lastname = q.Lastname,
                        PhoneNumber1 = q.PhoneNumber1,
                        PhoneNumber2 = q.PhoneNumber2,
                        PostalCode = q.PostalCode,
                    })
                    .ToListAsync(),
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

    public async Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters)
    {
        try
        {
            var keyword = parameters.Keyword.ToLower();
            var accounts = _accountRepository.DbSet().AsNoTracking()
                .Where(p => p.Email.ToLower().Contains(keyword) || p.Firstname.ToLower().Contains(keyword) ||
                            p.Lastname.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await accounts.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new GenericAccountViewModel()
                    {
                        AccountType = q.AccountType,
                        Address = q.Address,
                        Email = q.Email,
                        Firstname = q.Firstname,
                        Id = q.Id,
                        Lastname = q.Lastname,
                        PhoneNumber1 = q.PhoneNumber1,
                        PhoneNumber2 = q.PhoneNumber2,
                        PostalCode = q.PostalCode,
                    })
                    .ToListAsync(),
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
            var accountDetails = _mapper.Map<AccountDetailsViewModel>(account);

            accountDetails.AccountTypeName = account.AccountType.ToString();

            return new ResponseViewModel {Success = true, Payload = accountDetails};
        }
        catch (Exception e)
        {
            throw;
        }
    }
}