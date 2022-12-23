namespace LittleViet.Domain.Models;

public class Product : AuditableEntity
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public string StripeProductId { get; set; }
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; }
    public virtual ICollection<Serving> Servings { get; set; }
    public virtual ProductType ProductType { get; set; }
}

