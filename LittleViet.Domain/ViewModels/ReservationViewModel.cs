using System.Text.Json.Serialization;
using LittleViet.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Domain.ViewModels;

public class CreateReservationViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    public int NoOfPeople { get; set; }
    public DateTime BookingDate { get; set; }
    public string PhoneNumber { get; set; }
}

public class UpdateReservationViewModel
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    [JsonIgnore]
    public Guid? AccountId { get; set; }
    public int NoOfPeople { get; set; }
    public DateTime BookingDate { get; set; }
    public ReservationStatus Status { get; set; }
    public string PhoneNumber { get; set; }
}

public class ReservationViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    public int NoOfPeople { get; set; }
    public DateTime BookingDate { get; set; }
    public ReservationStatus Status { get; set; }
    public string StatusName { get; set; }
    public string PhoneNumber { get; set; }
}

public class ReservationDetailsViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    public int NoOfPeople { get; set; }
    public DateTime BookingDate { get; set; }
    public ReservationStatus Status { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
}

public class GetListReservationParameters : BaseListQueryParameters<Reservation>
{
    public GetListReservationParameters()
    {
        OrderBy = $"{nameof(Reservation.BookingDate)}";
    }
    
    public string FullName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    public int? NoOfPeople { get; set; }
    public DateTime? BookingDateFrom { get; set; }
    public DateTime? BookingDateTo { get; set; }
    public IEnumerable<ReservationStatus> Statuses { get; set; }
    public string PhoneNumber { get; set; }
}