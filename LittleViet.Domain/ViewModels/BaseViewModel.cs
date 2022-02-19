using System.Reflection;
using LittleViet.Data.Models;

namespace LittleViet.Data.ViewModels;

public class BaseSearchParameters : BaseListQueryParameters
{
    public string Keyword { get; set; }
}

public class BaseListQueryParameters
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string OrderBy { get; set; } = $"{nameof(AuditableEntity.CreatedDate)} desc";
}

public class BaseListQueryParameters<T> : BaseListQueryParameters where T : class
{
    public IEnumerable<string> Properties = typeof(T).GetProperties().Select(p => p.Name);
}

public class BaseListResponseViewModel : ResponseViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

public class BaseListResponseViewModel<T> : ResponseViewModel<T>
{
    public new List<T> Payload { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

public class ResponseViewModel<T> : ResponseViewModel
{
    public new bool Success { get; set; }
    public new string Message { get; set; }
    public new T Payload { get; set; }
}

public class ResponseViewModel
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Payload { get; set; }
}