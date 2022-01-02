using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Models.Repositories;
using LittleViet.Data.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrcyptNet = BCrypt.Net.BCrypt;

namespace LittleViet.Data.Domains;

public interface IAccountDomain
{
    ResponseViewModel Login(string email, string password);
    ResponseViewModel Deactivate(Guid id);
    ResponseViewModel Create(CreateAccountViewModel accountViewModel);
    ResponseViewModel Update(UpdateAccountViewModel accountViewModel);
    ResponseViewModel UpdatePassword(UpdatePasswordViewModel accountViewModel);
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

    public ResponseViewModel Login(string email, string password)
    {
        try
        {
            var account = _accRepo.GetActiveByEmail(email);
            if (account is null || !BrcyptNet.Verify(password, account.Password))
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
                            new Claim(ClaimTypes.Name, accountViewModel.Id.ToString()),
                            new Claim(ClaimTypes.Role, accountViewModel.AccountType.ToString())
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
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Create(CreateAccountViewModel createAccountViewModel)
    {
        try
        {
            var existedAccount = _accRepo.GetActiveByEmail(createAccountViewModel.Email);

            if (existedAccount != null)
            {
                return new ResponseViewModel { Success = false, Message = "This email already existed" };
            }
            var account = _mapper.Map<Account>(createAccountViewModel);

            var datetime = DateTime.UtcNow;

            account.Id = Guid.NewGuid();
            account.Password = BrcyptNet.HashPassword(createAccountViewModel.Password);
            account.IsDeleted = false;
            account.UpdatedDate = datetime;
            account.CreatedDate = datetime;
            account.UpdatedBy = createAccountViewModel.CreatedBy;

            _accRepo.Create(account);
            _uow.Save();

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Update(UpdateAccountViewModel updateAccountViewModel)
    {
        try
        {
            var existedAccount = _accRepo.GetActiveById(updateAccountViewModel.Id);

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

                _accRepo.Update(existedAccount);
                _uow.Save();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This account does not exist" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel)
    {
        try
        {


            if (updatePasswordViewModel.ConfirmPassword.Equals(updatePasswordViewModel.NewPassword))
            {
                var existedAccount = _accRepo.GetActiveById(updatePasswordViewModel.Id);



                if (existedAccount != null)
                {
                    if (!BrcyptNet.Verify(updatePasswordViewModel.OldPassword, existedAccount.Password))
                    {
                        return new ResponseViewModel { Message = "Wrong password", Success = false };
                    }

                    existedAccount.Password = BrcyptNet.HashPassword(updatePasswordViewModel.NewPassword);

                    _accRepo.Update(existedAccount);
                    _uow.Save();

                    return new ResponseViewModel { Success = true, Message = "Update successful" };
                }

                return new ResponseViewModel { Success = false, Message = "This account does not exist" };
            }

            return new ResponseViewModel { Success = false, Message = "New password and confirmation password do not match" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public ResponseViewModel Deactivate(Guid id)
    {
        try
        {
            var account = _accRepo.GetActiveById(id);
            _accRepo.DeactivateAccount(account);

            _uow.Save();
            return new ResponseViewModel { Message = "Delete successful", Success = true };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}

