using AutoMapper;
using LittleViet.Data.Models;
using LittleViet.Data.Models.Global;
using LittleViet.Data.Repositories;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Data.Domains;

public interface IOrderDomain
{
    Task<ResponseViewModel> Create(CreateOrderViewModel createOrderViewModel);
    Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel);
    Task<ResponseViewModel> Deactivate(Guid id);
    Task<BaseListQueryResponseViewModel> GetListOrders(BaseListQueryParameters parameters);
    Task<ResponseViewModel> GetOrderById(Guid id);
}
internal class OrderDomain : BaseDomain, IOrderDomain
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IMapper _mapper;

    public OrderDomain(IUnitOfWork uow, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IMapper mapper) : base(uow)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ResponseViewModel> Create(CreateOrderViewModel createOrderViewModel)
    {
        try
        {
            var order = _mapper.Map<Order>(createOrderViewModel);
            var datetime = DateTime.UtcNow;
            var orderGuid = Guid.NewGuid();

            order.Id = orderGuid;
            order.IsDeleted = false;
            order.OrderStatus = OrderStatus.Ordered;
            order.UpdatedDate = datetime;
            order.CreatedDate = datetime;
            order.UpdatedBy = createOrderViewModel.CreatedBy;

            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Id = Guid.NewGuid();
                orderDetail.OrderId = orderGuid;
                orderDetail.IsDeleted = false;
                orderDetail.UpdatedDate = datetime;
                orderDetail.CreatedDate = datetime;
                orderDetail.UpdatedBy = createOrderViewModel.CreatedBy;
                orderDetail.CreatedBy = createOrderViewModel.CreatedBy;
            }

            _orderRepository.Add(order);
            await _uow.SaveAsync();
            
            //create checkout session

            return new ResponseViewModel { Success = true, Message = "Create successful" };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Update(UpdateOrderViewModel updateOrderViewModel)
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
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> Deactivate(Guid id)
    {
        try
        {
            var order = await _orderRepository.DbSet().Include(t => t.OrderDetails.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(q => q.Id == id);

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
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseListQueryResponseViewModel> GetListOrders(BaseListQueryParameters parameters)
    {
        try
        {
            var order = _orderRepository.DbSet().AsNoTracking();

            return new BaseListQueryResponseViewModel
            {
                Payload = await order.Paginate(pageSize: parameters.PageSize, pageNum: parameters.PageNumber).ToListAsync(),
                Success = true,
                Total = await order.CountAsync(),
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
            };
        }
        catch (Exception e)
        {
            return new BaseListQueryResponseViewModel { Success = false, Message = e.Message };
        }
    }

    public async Task<ResponseViewModel> GetOrderById(Guid id)
    {
        try
        {
            var order = await _orderRepository.DbSet().Include(t => t.OrderDetails.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(q => q.Id == id);

            if (order == null)
            {
                return new ResponseViewModel { Success = false, Message = "This order does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = order };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
    
    public async Task<ResponseViewModel> Checkout(Guid id)
    {
        try
        {
            var order = await _orderRepository.DbSet().Include(t => t.OrderDetails.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(q => q.Id == id);

            if (order == null)
            {
                return new ResponseViewModel { Success = false, Message = "This order does not exist" };
            }

            return new ResponseViewModel { Success = true, Payload = order };
        }
        catch (Exception e)
        {
            return new ResponseViewModel { Success = false, Message = e.Message };
        }
    }
}