using AutoMapper;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Account;

public class AccountMappers : Profile
{
    public AccountMappers()
    {
            CreateMap<Models.Account, GenericAccountViewModel>().ReverseMap();
            CreateMap<Models.Account, CreateAccountViewModel>().ReverseMap();
            CreateMap<Models.Account, UpdateAccountViewModel>().ReverseMap();
            CreateMap<Models.Account, AccountDetailsViewModel>().ReverseMap();
    }
}