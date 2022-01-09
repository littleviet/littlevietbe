using LittleViet.Data.Models;
using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.ViewModels;

public class CreateReservationViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; } 
    public int NoOfPeople { get; set; }
    public DateTime BookingDate { get; set; }

    public ReservationStatus Status { get; set; }

    public Guid AccountId { get; set; }
}

public class UpdateReservationViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set;}

    public string LastName { get; set; }

    public string Email { get; set; }

    public string FurtherRequest { get; set; }

    public int  NoOfPeople { get; set; }

    public DateTime BookingDate { get; set; }

    public Guid UpdatedBy { get; set; }
}

public class SearchReservationViewModel : BaseListQueryParameters
{
     public Guid UpdatedBy { get; set; }

    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string FurtherRequest { get; set; }

    public int NoOfPeople { get; set; }

    public DateTime BookingDate { get; set; }
    public Guid CreatedBy { get; set; }
}