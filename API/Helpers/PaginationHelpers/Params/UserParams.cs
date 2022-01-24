using API.Helpers.Enums;

namespace API.Helpers.PaginationHelpers.Params;

public class UserParams : PaginationParams
{
    public string CurrentUsername { get; set; }
    public string Gender { get; set; }
    public Sexuality Sexuality { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 150;
    public string OrderBy { get; set; } = "lastActive";
}