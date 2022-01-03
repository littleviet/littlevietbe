namespace LittleViet.Data.ViewModels;

public class BaseSearchParameters : BaseListQueryParameters
{
    public string Keyword { get; set; }
}

public class BaseListQueryParameters
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 0;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}

public class BaseListQueryResponseViewModel : ResponseViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

public class ResponseViewModel
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Payload { get; set; }
}