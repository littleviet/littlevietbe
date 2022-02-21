using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Repositories;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.EntityFramework;
using static LittleViet.Infrastructure.EntityFramework.SqlHelper;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace LittleViet.Data.Domains.Order;

public interface IOrderDomain
{
    Task<ResponseViewModel> Create(CreateOrderViewModel createOrderViewModel);
    Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListOrders(GetListOrderParameters parameters);
    Task<ResponseViewModel> GetOrderById(Guid id);
    Task<ResponseViewModel> HandleSuccessfulOrder(Guid orderId, string stripeSessionId);
    Task<ResponseViewModel> HandleExpiredOrder(Guid orderId, string stripeSessionId);
    Task<BaseListResponseViewModel> Search(BaseSearchParameters parameters);
}

internal class OrderDomain : BaseDomain, IOrderDomain
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IMapper _mapper;

    public OrderDomain(IUnitOfWork uow, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository,
        IMapper mapper, IStripePaymentService stripePaymentService) : base(uow)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderDetailRepository =
            orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _stripePaymentService = stripePaymentService ?? throw new ArgumentNullException(nameof(stripePaymentService));
    }

    public async Task<ResponseViewModel> Create(CreateOrderViewModel createOrderViewModel)
    {
        var order = _mapper.Map<Models.Order>(createOrderViewModel);
        var orderGuid = Guid.NewGuid();

        order.Id = orderGuid;
        order.OrderStatus = OrderStatus.Ordered;

        foreach (var orderDetail in order.OrderDetails)
        {
            orderDetail.Id = Guid.NewGuid();
            orderDetail.OrderId = orderGuid;
        }

        await using var transaction = _uow.BeginTransation();

        try
        {
            _orderRepository.Add(order);
            await _uow.SaveAsync();

            var savedOrder = await _orderRepository.GetById(orderGuid);

            var stripeSessionDto = new CreateSessionDto()
            {
                Metadata = new() {{Infrastructure.Stripe.Payment.OrderCheckoutMetaDataKey, orderGuid.ToString()}},
                SessionItems = savedOrder.OrderDetails.Select(od => new SessionItem()
                {
                    StripePriceId = od.Serving.StripePriceId,
                    Quantity = od.Quantity,
                }).ToList()
            };

            var checkoutSessionResult = await _stripePaymentService.CreateOrderCheckoutSession(stripeSessionDto);

            order.LastStripeSessionId = checkoutSessionResult.Id;

            await _uow.SaveAsync();
            await transaction.CommitAsync();

            return new ResponseViewModel
            {
                Success = true,
                Message = "Create successful",
                Payload = new
                {
                    OrderId = orderGuid.ToString(),
                    Url = checkoutSessionResult.Url,
                    SessionId = checkoutSessionResult.Id,
                },
            };
        }
        catch (StripeException se)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel)
    {
        try
        {
            var existedOrder = await _orderRepository.GetById(updateOrderViewModel.Id);

            if (existedOrder != null)
            {
                existedOrder.PaymentType = updateOrderViewModel.PaymentType;
                existedOrder.PickupTime = updateOrderViewModel.PickupTime;
                existedOrder.TotalPrice = updateOrderViewModel.TotalPrice;
                existedOrder.OrderType = updateOrderViewModel.OrderType;

                _orderRepository.Modify(existedOrder);
                await _uow.SaveAsync();

                return new ResponseViewModel {Success = true, Message = "Update successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This order does not exist"};
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
            var order = await _orderRepository.DbSet().Include(t => t.OrderDetails.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(q => q.Id == id);

            if (order != null)
            {
                _orderRepository.Deactivate(order);

                foreach (var item in order.OrderDetails)
                {
                    _orderDetailRepository.Deactivate(item);
                }

                await _uow.SaveAsync();
                return new ResponseViewModel {Success = true, Message = "Delete successful"};
            }

            return new ResponseViewModel {Success = false, Message = "This order does not exist"};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel> GetListOrders(GetListOrderParameters parameters)
    {
        try
        {
            var orders = _orderRepository.DbSet().AsNoTracking()
                .ApplySort(parameters.OrderBy)
                .Include(q => q.Account)
                .WhereIf(parameters.AccountId is not null, o => o.AccountId == parameters.AccountId)                
                .WhereIf(!string.IsNullOrEmpty(parameters.FullName), 
                    ContainsIgnoreCase<Models.Order>(o => o.Account.Firstname + " " +  o.Account.Lastname,
                    parameters.FullName))
                .WhereIf(!string.IsNullOrEmpty(parameters.PhoneNumber), 
                    ContainsIgnoreCase<Models.Order>(o => o.Account.PhoneNumber1 + " " +  o.Account.PhoneNumber2, //TODO: improve this somehow
                    parameters.PhoneNumber))
                .WhereIf(parameters.Statuses is not null && parameters.Statuses.Any(),
                    o => parameters.Statuses.Contains(o.OrderStatus))
                .WhereIf(parameters.AccountId is not null, o => o.AccountId == parameters.AccountId)
                .WhereIf(parameters.PickupTimeTo is not null, o => o.PickupTime <= parameters.PickupTimeTo)
                .WhereIf(parameters.PickupTimeFrom is not null, o => o.PickupTime >= parameters.PickupTimeFrom)
                .WhereIf(parameters.TotalPriceTo is not null, o => o.TotalPrice <= parameters.TotalPriceTo)
                .WhereIf(parameters.TotalPriceFrom is not null, o => o.TotalPrice >= parameters.TotalPriceFrom)
                .WhereIf(parameters.OrderTypes is not null && parameters.OrderTypes.Any(),
                    o => parameters.OrderTypes.Contains(o.OrderType))
                .WhereIf(parameters.PaymentTypes is not null && parameters.PaymentTypes.Any(),
                    o => parameters.PaymentTypes.Contains(o.PaymentType));

            return new BaseListResponseViewModel
            {
                Payload = await orders
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new OrderViewModel()
                    {
                        Id = q.Id,
                        OrderType = q.OrderType,
                        PaymentType = q.PaymentType,
                        PickupTime = q.PickupTime,
                        TotalPrice = q.TotalPrice,
                        Status = q.OrderStatus,
                        Account = new GenericAccountViewModel()
                        {
                            AccountType = q.Account.AccountType,
                            Address = q.Account.Address,
                            Email = q.Account.Email,
                            Firstname = q.Account.Firstname,
                            Id = q.Account.Id,
                            Lastname = q.Account.Lastname,
                            PhoneNumber1 = q.Account.PhoneNumber1,
                            PhoneNumber2 = q.Account.PhoneNumber2,
                            PostalCode = q.Account.PostalCode,
                        }
                    })
                    .ToListAsync(),
                Success = true,
                Total = await orders.CountAsync(),
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
            var orders = _orderRepository.DbSet().Include(q => q.Account).AsNoTracking()
                .Where(p => p.Account.Firstname.ToLower().Contains(keyword) ||
                            p.Account.Lastname.ToLower().Contains(keyword) ||
                            p.Account.Email.ToLower().Contains(keyword)
                            || p.Account.PhoneNumber1.ToLower().Contains(keyword) ||
                            p.Account.PhoneNumber2.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await orders
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new OrderViewModel()
                    {
                        Id = q.Id,
                        OrderType = q.OrderType,
                        PaymentType = q.PaymentType,
                        PickupTime = q.PickupTime,
                        TotalPrice = q.TotalPrice,
                        Account = new GenericAccountViewModel()
                        {
                            AccountType = q.Account.AccountType,
                            Address = q.Account.Address,
                            Email = q.Account.Email,
                            Firstname = q.Account.Firstname,
                            Id = q.Account.Id,
                            Lastname = q.Account.Lastname,
                            PhoneNumber1 = q.Account.PhoneNumber1,
                            PhoneNumber2 = q.Account.PhoneNumber2,
                            PostalCode = q.Account.PostalCode,
                        }
                    })
                    .ToListAsync(),
                Success = true,
                Total = await orders.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> GetOrderById(Guid id)
    {
        try
        {
            var order = await _orderRepository.DbSet()
                .Include(t => t.OrderDetails.Where(p => p.IsDeleted == false))
                    .ThenInclude(od => od.Serving)
                        .ThenInclude(s => s.Product)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(q => q.Id == id) ?? throw new Exception("Order Id not found");
            
            var orderDetails = _mapper.Map<OrderDetailsViewModel>(order);

            orderDetails.OrderDetails = order.OrderDetails.Select(x => new OrderDetailItemViewModel()
            {
                Id = x.Id,
                ProductId = x.Serving.ProductId,
                ProductName = x.Serving.Product.Name,
                Price = x.Serving.Price,
                ServingId = x.ServingId,
                ServingName = x.Serving.Name,
                Quantity = x.Quantity,
            }).ToList();
            orderDetails.Account = _mapper.Map<GenericAccountViewModel>(order.Account);

            return new ResponseViewModel {Success = true, Payload = orderDetails};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> HandleSuccessfulOrder(Guid orderId, string stripeSessionId)
    {
        try
        {
            var order = await _orderRepository.DbSet().FirstOrDefaultAsync(q => q.Id == orderId);

            if (order == null)
            {
                throw new Exception($"Cannot find order of Id {order.Id}");
            }

            order.OrderStatus = OrderStatus.Paid;
            order.LastStripeSessionId = stripeSessionId;

            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true};
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> HandleExpiredOrder(Guid orderId, string stripeSessionId)
    {
        try
        {
            var order = await _orderRepository.DbSet().FirstOrDefaultAsync(q => q.Id == orderId);

            if (order == null)
            {
                throw new Exception($"Cannot find order of Id {order.Id}");
            }

            order.OrderStatus = OrderStatus.Expired;
            order.LastStripeSessionId = stripeSessionId;

            await _uow.SaveAsync();

            return new ResponseViewModel {Success = true};
        }
        catch (Exception e)
        {
            throw;
        }
    }
}