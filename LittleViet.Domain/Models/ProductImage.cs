namespace LittleViet.Domain.Models;

public class ProductImage : AuditableEntity
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
}
