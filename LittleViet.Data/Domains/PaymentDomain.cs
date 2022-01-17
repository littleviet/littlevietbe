using AutoMapper;
using LittleViet.Data.Models.Global;

namespace LittleViet.Data.Domains;

public interface IPaymentDomain
{
    
}

public class PaymentDomain : BaseDomain, IPaymentDomain
{
    private readonly IMapper _mapper;

    public PaymentDomain(IUnitOfWork uow, IMapper mapper) : base(uow)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public Task CreateSession()
    {
        throw new NotImplementedException();
    }


    public Task HandleOrderSuccess(string orderId)
    {
        throw new NotImplementedException();
        
    }
}

