using LittleViet.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Data.ViewModels;

public class CreateProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
    public int MainImage { get; set; }
    public List<IFormFile> ProductImages { get; set; }
}

public class UpdateProductViewModel
{
    [FromRoute]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public Guid ProductTypeId { get; set; }
}

public class ProductViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public string StatusName { get; set; }
    public string ProductTypeName { get; set; }
}

public class GetListProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public List<GenericServingViewModel> Servings { get; set; }
    public ProductStatus Status { get; set; }
    public string ImageUrl { get; set; }    
    public GetListProductTypeViewModel ProductType { get; set; }

    public class GetListProductTypeViewModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}

public class GetListProductParameters : BaseListQueryParameters<Product>
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public IEnumerable<ProductStatus> Statuses { get; set; }
    public Guid? ProductTypeId { get; set; }
}

public class ProductDetailsViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public GenericProductTypeViewModel ProductType { get; set; }
    public string StripeProductId { get; set; }
    public virtual ICollection<GenericProductImageViewModel> ProductImages { get; set; }
    public virtual ICollection<GenericServingViewModel> Servings { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
}

