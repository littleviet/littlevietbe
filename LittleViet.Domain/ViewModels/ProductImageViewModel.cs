using LittleViet.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Domain.ViewModels;

public class GenericProductImageViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
}

public class AddProductImagesViewModel
{
    [FromRoute]
    public Guid ProductId { get; set; }
    public List<IFormFile> ProductImages { get; set; }
}
