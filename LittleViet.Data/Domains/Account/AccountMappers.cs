using AutoMapper;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Account;

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