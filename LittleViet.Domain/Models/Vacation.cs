namespace LittleViet.Data.Models;

public class Vacation : AuditableEntity
{
    public DateTime Date { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}