using System.Text.Json.Serialization;
using LittleViet.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Data.ViewModels;

internal class ProductLandingPageViewModel
{
    public string Name { get; set; }
    public string CaName { get; set; }
    public string EsName { get; set; }
    public List<ProductsLandingPageViewModel> Products { get; set; }
}

internal class ProductsLandingPageViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public double? Price { get; set; }
}

public class CreateProductTypeViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
}

public class UpdateProductTypeViewModel
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
}

public class GetListProductTypeParameters : BaseListQueryParameters<ProductType>
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
}

public class ProductTypeItemViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public Guid Id { get; set; }
}

public class ProductTypeDetailsViewModel
{
    public string Name { get; set; }
    public string EsName { get; set; }
    public string CaName { get; set; }
    public string Description { get; set; }
    public Guid Id { get; set; }
    public List<ProductViewModel> Products { get; set; }
}

