using LittleViet.Data.Models;
using Microsoft.AspNetCore.Http;

namespace LittleViet.Data.ViewModels;

public class CreateProductImageViewModel
{
    public string Name { get; set; }
    public bool IsMain { get; set; }
}

public class GenericProductImageViewModel
{
    public Guid ProductId { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
}
