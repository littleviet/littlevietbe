using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

public class ProductImage : AuditableEntity
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
}
