namespace LittleViet.Infrastructure.Email.Models;

public class ReservationEmailDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string FurtherRequest { get; set; }
    public int NoOfPeople { get; set; }
}