namespace API.Helpers.PaginationHelpers;

public class PaginationParams
{
    private int _pageSize = 10;
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value < MaxPageSize) ? value : MaxPageSize;
    }
}