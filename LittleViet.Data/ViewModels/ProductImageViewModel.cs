using LittleViet.Data.Models;
using Microsoft.AspNetCore.Http;

namespace LittleViet.Data.ViewModels;

public class CreateProductImageViewModel
{
    public string Name { get; set; }
    public bool IsMain { get; set; }
}
