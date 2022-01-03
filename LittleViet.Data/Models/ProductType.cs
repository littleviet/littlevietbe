using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

public class ProductType : AuditableEntity
{
    public ProductType()
    {
        Products = new HashSet<Product>();
    }

    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}

