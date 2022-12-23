using LittleViet.Domain.Domains.Order;
using LittleViet.Domain.Domains.Reservations;
using LittleViet.Domain.ViewModels;

namespace LittleViet.Domain.Domains.Tasks;

public interface ITaskDomain
{
    public Task<TaskInitializeViewModel> Initialize();
}

internal class TaskDomain : ITaskDomain
{
    private readonly IReservationDomain _reservationDomain;
    private readonly IOrderDomain _orderDomain;

    public TaskDomain(IReservationDomain reservationDomain, IOrderDomain orderDomain)
    {
        _reservationDomain = reservationDomain;
        _orderDomain = orderDomain;
    }

    public async Task<TaskInitializeViewModel> Initialize()
    {
        return new TaskInitializeViewModel
        {
            NewReservationCount = await _reservationDomain.GetUnhandledReservationsCount(),
            NewPickupOrderCount = await _orderDomain.GetUnhandledOrdersCount()
        };
    }
}