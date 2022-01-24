namespace API.Helpers.PaginationHelpers.Params;

public class LikesParams : PaginationParams
{
    public int UserId { get; set; }
    public string Predicate { get; set; }
}