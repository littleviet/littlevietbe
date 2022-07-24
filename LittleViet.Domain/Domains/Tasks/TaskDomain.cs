using LittleViet.Data.Domains.Order;
using LittleViet.Data.Domains.Reservations;
using LittleViet.Data.ViewModels;

namespace LittleViet.Data.Domains.Tasks;

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