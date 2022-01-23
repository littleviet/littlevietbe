using AutoMapper;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using LittleViet.Infrastructure.Stripe.Interface;
using LittleViet.Infrastructure.Stripe.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Order = LittleViet.Data.Models.Order;

namespace LittleViet.Data.Domains;

public interface IOrderDomain
{
    Task<ResponseViewModel> Create(Guid userId, CreateOrderViewModel createOrderViewModel);
    Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListResponseViewModel> GetListOrders(BaseListQueryParameters parameters);
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

    public async Task<ResponseViewModel> Create(Guid userId, CreateOrderViewModel createOrderViewModel)
    {
        try
        {
            var order = _mapper.Map<Order>(createOrderViewModel);
            var now = DateTime.UtcNow;
            var orderGuid = Guid.NewGuid();

            order.Id = orderGuid;
            order.IsDeleted = false;
            order.OrderStatus = OrderStatus.Ordered;
            order.UpdatedDate = now;
            order.CreatedDate = now;
            order.UpdatedBy = userId;
            order.AccountId = userId;

            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Id = Guid.NewGuid();
                orderDetail.OrderId = orderGuid;
                orderDetail.IsDeleted = false;
                orderDetail.UpdatedDate = now;
                orderDetail.CreatedDate = now;
                orderDetail.UpdatedBy = userId;
                orderDetail.CreatedBy = userId;
            }

            _orderRepository.Add(order);
            await _uow.SaveAsync();

            var savedOrder = await _orderRepository.DbSet()
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Serving)
                .Where(o => o.Id == order.Id)
                .FirstOrDefaultAsync();

            var stripeSessionDto = new CreateSessionDto()
            {
                Metadata = new() { { "orderId", orderGuid.ToString() } },
                SessionItems = savedOrder.OrderDetails.Select(od => new SessionItem()
                {
                    StripePriceId = od.Serving.StripePriceId,
                    Quantity = od.Quantity,
                }).ToList()
            };
            var checkoutSessionResult = await _stripePaymentService.CreateCheckoutSession(stripeSessionDto);

            order.LastStripeSessionId = checkoutSessionResult.Id;

            await _uow.SaveAsync();

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
            throw;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel) //TODO: remove this as orders are immutable?
    {
        try
        {
            var existedOrder = await _orderRepository.GetById(updateOrderViewModel.Id);

            if (existedOrder != null)
            {
                existedOrder.UpdatedBy = updateOrderViewModel.UpdatedBy;
                existedOrder.PaymentType = updateOrderViewModel.PaymentType;
                existedOrder.PickupTime = updateOrderViewModel.PickupTime;
                existedOrder.TotalPrice = updateOrderViewModel.TotalPrice;
                existedOrder.OrderType = updateOrderViewModel.OrderType;
                existedOrder.UpdatedDate = DateTime.UtcNow;

                _orderRepository.Modify(existedOrder);
                await _uow.SaveAsync();

                return new ResponseViewModel { Success = true, Message = "Update successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This order does not exist" };
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
                return new ResponseViewModel { Success = true, Message = "Delete successful" };
            }

            return new ResponseViewModel { Success = false, Message = "This order does not exist" };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<BaseListResponseViewModel> GetListOrders(BaseListQueryParameters parameters)
    {
        try
        {
            var orders = _orderRepository.DbSet().Include(q => q.Account).AsNoTracking();

            return new BaseListResponseViewModel
            {
                Payload = await orders.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                    .Select(q => new OrderViewModel()
                    {
                        Id = q.Id,
                        OrderType = q.OrderType,
                        OrderTypeName = q.OrderType.ToString(),
                        PaymentType = q.PaymentType,
                        PaymentTypeName = q.PaymentType.ToString(),
                        PickupTime = q.PickupTime,
                        TotalPrice = q.TotalPrice,
                        Account = new AccountViewModel()
                        {
                            AccountType = q.Account.AccountType,
                            AccountTypeName = q.Account.AccountType.ToString(),
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
                .Where(p => p.Account.Firstname.ToLower().Contains(keyword) || p.Account.Lastname.ToLower().Contains(keyword) || p.Account.Email.ToLower().Contains(keyword)
                || p.Account.PhoneNumber1.ToLower().Contains(keyword) || p.Account.PhoneNumber2.ToLower().Contains(keyword));

            return new BaseListResponseViewModel
            {
                Payload = await orders
                    .Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber)
                     .Select(q => new OrderViewModel()
                     {
                         Id = q.Id,
                         OrderType = q.OrderType,
                         OrderTypeName = q.OrderType.ToString(),
                         PaymentType = q.PaymentType,
                         PaymentTypeName = q.PaymentType.ToString(),
                         PickupTime = q.PickupTime,
                         TotalPrice = q.TotalPrice,
                         Account = new AccountViewModel()
                         {
                             AccountType = q.Account.AccountType,
                             AccountTypeName = q.Account.AccountType.ToString(),
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
            var order = await _orderRepository.DbSet().Include(t => t.OrderDetails.Where(p => p.IsDeleted == false))
                .Include(p => p.Account)
                .FirstOrDefaultAsync(q => q.Id == id);

            var orderDetails = _mapper.Map<OrderDetailsViewModel>(order);

            orderDetails.PaymentTypeName = order.PaymentType.ToString();
            orderDetails.OrderTypeName = order.OrderType.ToString();
            orderDetails.OrderDetails = _mapper.Map<List<OrderDetailViewModel>>(order.OrderDetails);
            orderDetails.Account = _mapper.Map<AccountViewModel>(order.Account);
            orderDetails.Account.AccountTypeName = order.Account.AccountType.ToString();

            if (order == null)
            {
                return new ResponseViewModel { Success = false, Message = "This order does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = orderDetails };
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

            return new ResponseViewModel { Success = true };
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

            return new ResponseViewModel { Success = true };
        }
        catch (Exception e)
        {
            throw;
        }
    }
}